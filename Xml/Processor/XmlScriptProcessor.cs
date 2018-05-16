using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer.Abstract;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Xml.Processor
{
  public static class XmlScriptProcessor
  {
    private static readonly Dictionary<string, bool> _executeTaskList = new Dictionary<string, bool>();

    private static readonly object _executeTaskListLock = new object();

    /// <summary>
    ///   Überprüft, ob es sich bei der übergebenen Datei (path) um ein CEScript handelt.
    /// </summary>
    /// <param name="path">Datei</param>
    /// <returns><c>true</c> wenn es sich um ein CEScript handelt, andernfalls <c>false</c>.</returns>
    public static bool IsXmlScript(string path)
    {
      try
      {
        var lines = File.ReadAllLines(path, Configuration.Encoding);
        var max = lines.Length < 5 ? lines.Length : 5;
        for (var i = 0; i < max; i++)
          if (lines[i].ToLower().Contains("<cescript>"))
            return true;

        return false;
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    ///   Verarbeitete ein CEScript
    /// </summary>
    /// <param name="path">Pfad des CEScript</param>
    /// <param name="actions">Actions - Auflistung wird in CorpusExplorer.Terminal.Console.Program festgelegt.</param>
    /// <param name="formats">
    ///   Formate für Tabellenexport - Auflistung wird in CorpusExplorer.Terminal.Console.Program
    ///   festgelegt.
    /// </param>
    public static void Process(string path, Dictionary<string, AbstractAction> actions,
      Dictionary<string, AbstractTableWriter> formats)
    {
      var script = LoadCeScript(path, out var scriptFilename);
      if (script == null)
      {
        System.Console.WriteLine("XML Parser Error 001");
        return;
      }

      ConsoleHelper.PrintHeader();

      Parallel.ForEach(script.sessions, Configuration.ParallelOptions, session =>
      {
        HashSet<string> deletePaths = null;
        try
        {
          var source = ReadSources(session.sources, out deletePaths);
          var selections = GenerateSelections(source, session.queries);
          var allGuid = source.SelectAll.Guid;

          Parallel.ForEach(session.tasks, Configuration.ParallelOptions, task =>
          {
            try
            {
              if (!actions.ContainsKey(task.type))
                return;
              var action = actions[task.type];

              // Wenn eine Action alle Queries adressiert (query="*") dann durchlaufe alle Queries.
              // + adressiert alle erstellten Queires und nicht ALL
              if (task.query == "*" || task.query == "+")
              {
                Parallel.ForEach(selections, Configuration.ParallelOptions, selection =>
                {
                  if (task.query == "+" && selection.Value.First().Guid == allGuid)
                    return;

                  Parallel.ForEach(selection.Value, sel =>
                  {
                    try
                    {
                      ExecuteTask(action, task, formats, sel, scriptFilename);
                    }
                    catch (Exception ex)
                    {
                      // ignore
                    }
                  });
                });
              }
              // Wird nur ein bestimmter Query adressiert, dann werte nur diesen aus.
              else
              {
                if (!selections.ContainsKey(task.query ?? string.Empty))
                  return;

                var selection = selections[task.query ?? string.Empty];
                Parallel.ForEach(selection, sel => { ExecuteTask(action, task, formats, sel, scriptFilename); });
              }
            }
            catch (Exception ex)
            {
              // ignore
            }
          });
        }
        catch
        {
          // ignore
        }
        // Bereinige nicht mehr benötigte Dateien
        finally
        {
          try
          {
            if (deletePaths != null)
              foreach (var p in deletePaths)
                try
                {
                  File.Delete(p);
                }
                catch
                {
                  // ignore
                }
          }
          catch
          {
            // ignore
          }
        }
      });
    }

    /// <summary>
    ///   Führe den Task aus.
    /// </summary>
    /// <param name="action">Action - siehe CorpusExplorer.Terminal.Console.Action</param>
    /// <param name="task">Task (beinhaltet Information zum Ausführen und Speichern der Resulate)</param>
    /// <param name="formats">Ausgabeformat</param>
    /// <param name="selection">Schnappschuss</param>
    /// <param name="scriptFilename">Name des CeScripts</param>
    private static void ExecuteTask(AbstractAction action, task task, Dictionary<string, AbstractTableWriter> formats,
      Selection selection, string scriptFilename)
    {
      try
      {
        var outputPath = OutputPathBuilder(task.output.Value, scriptFilename, selection.Displayname, task.type);

        // Wurde der Task bereits abgeschlossen? - Falls ja, breche ab.
        if (File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
          return;

        // Reporting für Konsole
        ExecuteTaskReport(selection.Displayname, task.type, outputPath, false);

        // Ist der Task vom Typ query oder convert, dann muss ist format ein AbstractExporter
        if (task.type == "query" || task.type == "convert")
        {
          var exporters = Configuration.AddonExporters.GetDictionary();
          if (!exporters.ContainsKey(task.output.format))
            return;

          exporters[task.output.format].Export(selection, outputPath);
        }
        // Andernfalls ist format ein AbstractTableWriter
        else
        {
          var formatKey = task.output.format.StartsWith("F:") ? task.output.format : $"F:{task.output.format}";
          if (!formats.ContainsKey(formatKey))
            return;

          // Kopie des TableWriter, um eine parallele Verarbeitung zu ermöglichen.
          var format = Activator.CreateInstance(formats[formatKey].GetType()) as AbstractTableWriter;
          using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
          using (var bs = new BufferedStream(fs))
          {
            format.OutputStream = bs;
            action.Execute(selection, task.arguments, format);
          }
        }

        // Reporting für Konsole
        ExecuteTaskReport(selection.Displayname, task.type, outputPath, true);
      }
      catch (Exception ex)
      {
        try
        {
          File.AppendAllLines("error.log",
            new[] {$"{selection.Displayname} - {action.Action} - {task.type}", ex.Message, ex.StackTrace});
        }
        catch
        {
          // ignore
        }
      }
    }

    /// <summary>
    ///   Erzeugt eine Ausgabe auf der Konsole - Damit die Nutzer*in einen Überblick behält was aktuell passiert.
    /// </summary>
    /// <param name="selectionDisplayname">Schnappschussname</param>
    /// <param name="taskType">Typ der Aufgabe</param>
    /// <param name="outputPath">Pfad der Ausgabedatei</param>
    /// <param name="done">Ist die Aufgabe erledigt?</param>
    private static void ExecuteTaskReport(string selectionDisplayname, string taskType, string outputPath, bool done)
    {
      try
      {
        var key = $"{selectionDisplayname} > {taskType} > {Path.GetFileName(outputPath)}";
        lock (_executeTaskListLock)
        {
          // Entferne bereits erledigte Aufgaben
          var keys = _executeTaskList.Keys.ToArray();
          foreach (var k in keys)
            if (_executeTaskList[k])
              _executeTaskList.Remove(k);

          // Status aktualisieren
          if (_executeTaskList.ContainsKey(key))
            _executeTaskList[key] = done;
          else
            _executeTaskList.Add(key, done);

          // Liste ausgeben
          System.Console.ForegroundColor = ConsoleColor.Gray;
          ConsoleHelper.PrintHeader();
          System.Console.WriteLine("..:: CURRENT TASKS ::..");
          foreach (var t in _executeTaskList)
          {
            System.Console.ForegroundColor = t.Value ? ConsoleColor.Green : ConsoleColor.Yellow;
            System.Console.WriteLine($"{t.Key} ... {(t.Value ? "done" : "running")}");
          }

          System.Console.ForegroundColor = ConsoleColor.Gray;
        }
      }
      catch
      {
        // ignore
      }
    }

    /// <summary>
    ///   Erzeugt Abfragen
    /// </summary>
    /// <param name="source">Projekt</param>
    /// <param name="queries">Abfragen</param>
    /// <returns>Auflistung mit allen Abfragen</returns>
    private static Dictionary<string, Selection[]> GenerateSelections(Project source, queries queries)
    {
      var all = source.SelectAll;
      all.Displayname = "ALL";
      var res = new Dictionary<string, Selection[]> {{"", new[] {all}}};

      foreach (var item in queries.Items)
        try
        {
          switch (item)
          {
            case query q:
              GenerateSelections_SingleQuery(q, ref res, source.SelectAll);
              break;
            case queryBuilder b:
              GenerateSelections_QueryBuilder(b, ref res, source.SelectAll);
              break;
            case queryGroup g:
              GenerateSelections_QueryGroup(g, ref res, source.SelectAll);
              break;
          }
        }
        catch
        {
          // ignore
        }

      return res;
    }

    /// <summary>
    ///   Nutzt den QueryParser des CE, um Abfragen zu bauen
    /// </summary>
    /// <param name="selection">Schnappschuss: Alle Dokumente</param>
    /// <param name="query">Abfrage</param>
    /// <param name="key">Name der Abfrage</param>
    /// <returns>Neue Schnappschüsse</returns>
    private static Selection[] GenerateSelections_Compile(Selection selection, string query, string key)
    {
      try
      {
        var filterQuery = QueryParser.Parse(query.CleanXmlValue());
        if (!(filterQuery is FilterQueryUnsupportedParserFeature))
          return new[] {selection.Create(new[] {filterQuery}, key)};

        var q = (FilterQueryUnsupportedParserFeature) filterQuery;
        switch (q.MetaLabel)
        {
          case "<:RANDOM:>":
            return GenerateSelections_RandomSplit(selection, q.MetaValues);
          case "<:CORPUS:>":
            return GenerateSelections_CorporaSplit(selection);
          default:
            return GenerateSelections_MetaSplit(selection, q, q.MetaValues);
        }
      }
      catch
      {
        // ignore
      }

      return null;
    }

    /// <summary>
    ///   Ermöglicht es auf alle Korpora zuzugreifen
    /// </summary>
    /// <param name="selection">Schnappschuss</param>
    /// <returns>Neue Schnappschüsse</returns>
    private static Selection[] GenerateSelections_CorporaSplit(Selection selection)
    {
      return
        (from csel in selection.CorporaGuids
          let corpus = selection.GetCorpus(csel)
          let dsels = new HashSet<Guid>(corpus.DocumentGuids)
          select selection.Create(new Dictionary<Guid, HashSet<Guid>> {{csel, dsels}}, corpus.CorpusDisplayname))
        .ToArray();
    }

    /// <summary>
    ///   Ermöglicht eine Metasplit-Abfrage (wird vom QueryParser nicht unterstützt).
    /// </summary>
    /// <param name="selection">Schnappschuss</param>
    /// <param name="q">Abfrage</param>
    /// <param name="values">Parameter</param>
    /// <returns>Neue Schnappschüsse</returns>
    private static Selection[] GenerateSelections_MetaSplit(Selection selection, FilterQueryUnsupportedParserFeature q,
      IEnumerable<object> values)
    {
      var vs = values?.ToArray();
      if (vs?.Length != 1)
        return null;

      var block = AutoSplitBlockHelper.RunAutoSplit(selection, q, vs);
      return block.GetSelectionClusters().ToArray();
    }

    /// <summary>
    ///   Baut aus vorgegebenen Werten mehrere Einzelabfragen
    /// </summary>
    /// <param name="queryBuilder">QueryBuilder</param>
    /// <param name="res">Rückgabeliste</param>
    /// <param name="all">Schnappschuss: Alle Dokumente</param>
    private static void GenerateSelections_QueryBuilder(queryBuilder queryBuilder,
      ref Dictionary<string, Selection[]> res, Selection all)
    {
      var key = queryBuilder.name ?? string.Empty;
      if (key == "" || key == "*" || key == "+" || queryBuilder.value == null)
        return;
      foreach (var v in queryBuilder.value)
      {
        var gname = (queryBuilder.name + v).Replace("\"", "");
        if (res.ContainsKey(gname))
          continue;
        var gquery = queryBuilder.prefix + v;

        res.Add(gname, GenerateSelections_Compile(all, gquery, gname));
      }
    }

    /// <summary>
    ///   Eine QueryGroup verknüpft mehrere Abfragen miteinander
    /// </summary>
    /// <param name="queryGroup">QueryGroup</param>
    /// <param name="res">Rückgabeliste</param>
    /// <param name="all">Schnappschuss: Alle Dokumente</param>
    private static void GenerateSelections_QueryGroup(queryGroup queryGroup, ref Dictionary<string, Selection[]> res,
      Selection all)
    {
      var key = queryGroup.name ?? string.Empty;
      if (key == "" || key == "*" || key == "+" || res.ContainsKey(key))
        return;

      // Erzeuge erste Abfrage
      var qs = new List<query>(queryGroup.query);
      var selection = GenerateSelections_Compile(all, qs[0].Text.CleanXmlValue(), qs[0].name).First()
        .CorporaAndDocumentGuids.ToDictionary(x => x.Key, x => new HashSet<Guid>(x.Value));
      qs.RemoveAt(0); // Entferne erste Abfrage aus der Liste

      // Führe alle Folgeabfragen aus.
      foreach (var query in qs)
      {
        var temp = GenerateSelections_Compile(all.CreateTemporary(selection), query.Text.CleanXmlValue(), "").First()
          .CorporaAndDocumentGuids.ToDictionary(x => x.Key, x => new HashSet<Guid>(x.Value));
        switch (queryGroup.@operator)
        {
          default:
          case "and": // Ergebnisse müssen mit allen Abfragen übereinstimmen
            var csels = selection.Keys.ToArray();
            foreach (var csel in csels)
            {
              if (!temp.ContainsKey(csel))
              {
                selection.Remove(csel);
                continue;
              }

              var dsels = selection[csel];
              foreach (var dsel in dsels)
                if (!temp.ContainsKey(dsel))
                  selection[csel].Remove(dsel);
            }

            break;
          case "or": // Ergebnis trifft auf die erste oder eine Folgeabfrage zu
            foreach (var csel in temp)
            {
              if (!selection.ContainsKey(csel.Key))
                selection.Add(csel.Key, new HashSet<Guid>());
              foreach (var dsel in csel.Value)
                if (!selection[csel.Key].Contains(dsel))
                  selection[csel.Key].Add(dsel);
            }

            break;
        }
      }

      res.Add(key, new[] {all.Create(selection, key)});
    }

    /// <summary>
    ///   Ermöglicht es einen zufälligen Schnappschuss zu erstellen
    /// </summary>
    /// <param name="selection">Schnappschuss</param>
    /// <param name="values">Parameter</param>
    /// <returns>Neue Schnappschüsse</returns>
    private static Selection[] GenerateSelections_RandomSplit(Selection selection, IEnumerable<object> values)
    {
      var block = selection.CreateBlock<RandomSelectionBlock>();
      block.DocumentCount = int.Parse(values.First().ToString());
      block.Calculate();
      return new[] {block.RandomSelection, block.RandomInvertSelection};
    }

    /// <summary>
    ///   Erzeugt eine Einzelabfrage
    /// </summary>
    /// <param name="query">Einzelabfrage</param>
    /// <param name="res">Rückgabeliste</param>
    /// <param name="all">Schnappschuss: Alle Dokumente</param>
    private static void GenerateSelections_SingleQuery(query query, ref Dictionary<string, Selection[]> res,
      Selection all)
    {
      var key = query.name ?? string.Empty;
      if (key == "" || key == "*" || key == "+" || res.ContainsKey(key))
        return;
      res.Add(key, GenerateSelections_Compile(all, query.Text.CleanXmlValue(), query.name));
    }

    /// <summary>
    ///   Lade/Deserialisere das CeScrpit
    /// </summary>
    /// <param name="path">Pfad</param>
    /// <param name="scriptFilename">Gibt den Dateinamen ohne Erweiterung zurück</param>
    /// <returns>CeScript</returns>
    private static cescript LoadCeScript(string path, out string scriptFilename)
    {
      cescript script = null;
      scriptFilename = Path.GetFileNameWithoutExtension(path);
      try
      {
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
          var se = new XmlSerializer(typeof(cescript));
          script = se.Deserialize(fs) as cescript;
        }
      }
      catch (Exception ex)
      {
        // ignore
      }

      return script;
    }

    /// <summary>
    ///   Erzeugt einen Ausgabepfad
    /// </summary>
    /// <param name="path">Pfad</param>
    /// <param name="scriptFilename">Name des CeScripts</param>
    /// <param name="selectionName">Schnappschussname</param>
    /// <param name="action">Action</param>
    /// <returns>Ausgabepfad</returns>
    private static string OutputPathBuilder(string path, string scriptFilename, string selectionName, string action)
    {
      var res = path.Replace("{all}", "{script}_{selection}_{action}").Replace("{script}", scriptFilename)
        .Replace("{selection}", selectionName == "*" ? "ALL" : selectionName).Replace("{action}", action)
        .EnsureFileName();
      var dir = Path.GetDirectoryName(res);
      if (dir != null && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      return res;
    }

    /// <summary>
    ///   Liest die gewünschten Korpusquellen ein
    /// </summary>
    /// <param name="sources">Quellen</param>
    /// <param name="deletePaths">Dateien die nach Ende der Session gelöscht werden sollen</param>
    /// <returns>Project</returns>
    private static Project ReadSources(sources sources, out HashSet<string> deletePaths)
    {
      var res = CorpusExplorerEcosystem.InitializeMinimal();
      deletePaths = new HashSet<string>();

      // Wenn zu annotierendes Material vorhanden ist, dann lese dieses ein.
      if (sources.annotate != null)
      {
        var scrapers = Configuration.AddonScrapers.GetDictionary();
        var taggers = Configuration.AddonTaggers.GetDictionary();

        foreach (var annotate in sources.annotate)
          try
          {
            if (!scrapers.ContainsKey(annotate.type))
              continue;
            if (!taggers.ContainsKey(annotate.tagger))
              continue;

            // Extrahiere und bereinige die Dokumente
            var scraper = scrapers[annotate.type];
            scraper.Input.Enqueue(SearchFiles(annotate.Items, ref deletePaths));
            scraper.Execute();
            var cleaner1 = new StandardCleanup {Input = scraper.Output};
            cleaner1.Execute();
            var cleaner2 = new RegexXmlMarkupCleanup {Input = cleaner1.Output};
            cleaner2.Execute();

            // Annotiere das Textmaterial
            var tagger = taggers[annotate.tagger];
            tagger.LanguageSelected = annotate.language;
            tagger.Input = cleaner2.Output;
            tagger.Execute();

            foreach (var corpus in tagger.Output)
              res.Add(corpus);
          }
          catch
          {
            // ignore
          }
      }

      // Wenn Import-Quellen vorhanden sind, dann lese diese ein.
      // ReSharper disable once InvertIf
      if (sources.import != null)
      {
        var importers = Configuration.AddonImporters.GetDictionary();
        foreach (var import in sources.import)
          try
          {
            if (!importers.ContainsKey(import.type))
              continue;

            foreach (var corpus in importers[import.type].Execute(SearchFiles(import.Items, ref deletePaths)))
              res.Add(corpus);
          }
          catch
          {
            //ignore
          }
      }

      return res;
    }

    /// <summary>
    ///   Es gibt zwei mögliche Arten Quellen zu spezifizieren - als Datei oder komplette Order
    /// </summary>
    /// <param name="annotateItems">Quellen</param>
    /// <param name="deletePaths">Dateien die nach Ende der Session gelöscht werden sollen</param>
    /// <returns>Dateien</returns>
    private static IEnumerable<string> SearchFiles(object[] annotateItems, ref HashSet<string> deletePaths)
    {
      var res = new List<string>();

      foreach (var item in annotateItems)
        try
        {
          switch (item)
          {
            case file i:
              res.Add(i.Value);
              if (i.delete)
                deletePaths.Add(i.Value);
              break;
            case directory i:
              var files = Directory.GetFiles(i.Value, i.filter, SearchOption.TopDirectoryOnly);
              res.AddRange(files);
              if (i.delete)
                foreach (var f in files)
                  deletePaths.Add(f);
              break;
          }
        }
        catch
        {
          // ignore
        }

      return res;
    }
  }
}