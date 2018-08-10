using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Xml.Extensions;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Xml.Processor
{
  public static class XmlScriptProcessor
  {
    private static string _errorLog;
    private static readonly Dictionary<string, bool> _executeActionList = new Dictionary<string, bool>();
    private static readonly object _executeActionListLock = new object();
    private static readonly object _sourceLoadLock = new object();

    /// <summary>
    ///   Verarbeitete ein CEScript
    /// </summary>
    /// <param name="path">Pfad des CEScript</param>
    /// <param name="actions">Actions - Auflistung wird in CorpusExplorer.Terminal.Console.Program festgelegt.</param>
    /// <param name="formats">
    ///   Formate für Tabellenexport - Auflistung wird in CorpusExplorer.Terminal.Console.Program
    ///   festgelegt.
    /// </param>
    public static void Process(string path, Dictionary<string, IAddonConsoleAction> actions,
                               Dictionary<string, AbstractTableWriter> formats)
    {
      _errorLog = path + ".log";
      var script = LoadCeScript(path, out var scriptFilename);
      if (script == null)
      {
        System.Console.WriteLine("XML Parser Error 001");
        return;
      }

      ConsoleHelper.PrintHeader();

      if (!string.IsNullOrEmpty(script.sessions.mode) && script.sessions.mode.StartsWith("sync"))
        foreach (var session in script.sessions.session)
          ExecuteSession(actions, formats, session, scriptFilename);
      else
        Parallel.ForEach(script.sessions.session, Configuration.ParallelOptions,
                         session => { ExecuteSession(actions, formats, session, scriptFilename); });
    }

    private static void ExecuteSession(Dictionary<string, IAddonConsoleAction> actions,
                                       Dictionary<string, AbstractTableWriter> formats, session session,
                                       string scriptFilename)
    {
      HashSet<string> deletePaths = null;
      try
      {
        Project source;
        lock (_sourceLoadLock)
        {
          source = ReadSources(session.sources, out deletePaths);
        }

        var selections = GenerateSelections(source, session.queries);
        var allowOverride = session.@override;

        if (!string.IsNullOrEmpty(session.actions.mode) && session.actions.mode.StartsWith("sync"))
          foreach (var action in session.actions.action)
            ExecuteSessionAction(actions, formats, scriptFilename, action, selections, allowOverride);
        else
          Parallel.ForEach(session.actions.action, Configuration.ParallelOptions,
                           action =>
                           {
                             ExecuteSessionAction(actions, formats, scriptFilename, action, selections, allowOverride);
                           });
      }
      catch (Exception ex)
      {
        LogError(ex);
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
              catch (Exception ex)
              {
                LogError(ex);
              }
        }
        catch (Exception ex)
        {
          LogError(ex);
        }
      }
    }

    private static void ExecuteSessionAction(Dictionary<string, IAddonConsoleAction> actions,
                                             Dictionary<string, AbstractTableWriter> formats, string scriptFilename,
                                             action a,
                                             Dictionary<string, Selection[]> selections, bool allowOverride)
    {
      try
      {
        if (!actions.ContainsKey(a.type))
          return;
        var action = actions[a.type];

        var query = a.query ?? string.Empty;
        var actionSelections = new List<Selection>();
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (query == "*") // Alle Queries
        {
          actionSelections.AddRange(selections.SelectMany(x => x.Value));
        }
        else if (query == "+") // Alle Queries außer SELECTALL
        {
          var first = selections.First().Key;
          actionSelections.AddRange(selections.Where(x => x.Key != first).SelectMany(x => x.Value));
        }
        else if (query.StartsWith("*")) // Alle Queries die auf query enden
        {
          var q = query.Substring(1);
          foreach (var x in selections)
            if (x.Key.EndsWith(q))
              actionSelections.AddRange(x.Value);
        }
        else if (query.EndsWith("*")) // Alle Queries die auf query beginnen
        {
          var q = query.Substring(0, query.Length - 1);
          foreach (var x in selections)
            if (x.Key.StartsWith(q))
              actionSelections.AddRange(x.Value);
        }
        else if (!selections.ContainsKey(query)) // Wenn kein Query verfügbar breche ab
        {
          return;
        }
        else // Einzelquery
        {
          actionSelections.AddRange(selections[query]);
        }

        try
        {
          ExecuteAction(action, a, formats, actionSelections, query, scriptFilename, allowOverride);
        }
        catch (Exception ex)
        {
          LogError(ex);
        }
      }
      catch (Exception ex)
      {
        LogError(ex);
      }
    }

    /// <summary>
    ///   Führe eine Action aus.
    /// </summary>
    /// <param name="action">Action - siehe CorpusExplorer.Terminal.Console.Action</param>
    /// <param name="a">Action (beinhaltet Information zum Ausführen und Speichern der Resulate)</param>
    /// <param name="formats">Ausgabeformat</param>
    /// <param name="selections">Schnappschüsse</param>
    /// <param name="query">Query-Pattern, das zur auswahl der Schnappschüsse dient</param>
    /// <param name="scriptFilename">Name des CeScripts</param>
    /// <param name="allowOverride">Erlaubt das Überschreiben von exsistierenden Ausgabedateien</param>
    private static void ExecuteAction(IAddonConsoleAction action, action a,
                                      Dictionary<string, AbstractTableWriter> formats,
                                      List<Selection> selections, string query, string scriptFilename,
                                      bool allowOverride)
    {
      try
      {
        if (!string.IsNullOrEmpty(a.mode) && a.mode == "merge")
        {
          var outputPath = OutputPathBuilder(a.output.Value, scriptFilename, query, a.type);

          // Wurde eine Action bereits abgeschlossen? - Falls ja, breche ab.
          if (!allowOverride && File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
            return;

          // Reporting für Konsole
          ExecuteActionReport(query, a.type, outputPath, false);

          // Ist die Action vom Typ query oder convert, dann muss ist format vom Typ AbstractExporter
          if (a.type == "query" || a.type == "convert")
          {
            var exporters = Configuration.AddonExporters.GetDictionary();
            if (!exporters.ContainsKey(a.output.format))
              return;

            exporters[a.output.format]
             .Export(selections.JoinFull(Path.GetFileNameWithoutExtension(outputPath)).ToCorpus(), outputPath);
          }
          // Andernfalls ist format vom Typ AbstractTableWriter
          else
          {
            var formatKey = a.output.format.StartsWith("F:") ? a.output.format : $"F:{a.output.format}";
            if (!formats.ContainsKey(formatKey) || formats[formatKey] == null)
              return;

            // Kopie des TableWriter, um eine parallele Verarbeitung zu ermöglichen.
            if (Activator.CreateInstance(formats[formatKey].GetType()) is AbstractTableWriter format)
              using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
              using (var bs = new BufferedStream(fs))
              {
                format.OutputStream = bs;
                Parallel.ForEach(selections, Configuration.ParallelOptions,
                                 // ReSharper disable once AccessToDisposedClosure
                                 // ReSharper disable once ImplicitlyCapturedClosure
                                 selection => action.Execute(selection, a.arguments, format));
                format.Destroy();
              }
          }

          // Reporting für Konsole
          ExecuteActionReport(query, a.type, outputPath, true);
        }
        else
        {
          Parallel.ForEach(selections, Configuration.ParallelOptions, selection =>
          {
            var outputPath = OutputPathBuilder(a.output.Value, scriptFilename, selection.Displayname, a.type);

            // Wurde die Action bereits abgeschlossen? - Falls ja, breche ab.
            if (!allowOverride && File.Exists(outputPath) && new FileInfo(outputPath).Length > 0)
              return;

            // Reporting für Konsole
            ExecuteActionReport(selection.Displayname, a.type, outputPath, false);

            // Ist die Action vom Typ query oder convert, dann muss ist format vom Typ AbstractExporter
            if (a.type == "query" || a.type == "convert")
            {
              var exporters = Configuration.AddonExporters.GetDictionary();
              if (!exporters.ContainsKey(a.output.format))
                return;

              exporters[a.output.format].Export(selection.ToCorpus(), outputPath); // ToDo: Multi-File outout
            }
            // Andernfalls ist format vom Typ AbstractTableWriter
            else
            {
              var formatKey = a.output.format.StartsWith("F:") ? a.output.format : $"F:{a.output.format}";
              if (!formats.ContainsKey(formatKey) || formats[formatKey] == null)
                return;

              // Kopie des TableWriter, um eine parallele Verarbeitung zu ermöglichen.
              if (Activator.CreateInstance(formats[formatKey].GetType()) is AbstractTableWriter format)
                using (var fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                using (var bs = new BufferedStream(fs))
                {
                  format.OutputStream = bs;
                  action.Execute(selection, a.arguments, format);
                  format.Destroy();
                }
            }

            // Reporting für Konsole
            ExecuteActionReport(selection.Displayname, a.type, outputPath, true);
          });
        }
      }
      catch (Exception ex)
      {
        LogError(ex, $"{query} - {action.Action} - {a.type}");
      }
    }

    /// <summary>
    ///   Erzeugt eine Ausgabe auf der Konsole - Damit die Nutzer*in einen Überblick behält was aktuell passiert.
    /// </summary>
    /// <param name="selectionDisplayname">Schnappschussname</param>
    /// <param name="actionType">Typ der Aufgabe</param>
    /// <param name="outputPath">Pfad der Ausgabedatei</param>
    /// <param name="done">Ist die Aufgabe erledigt?</param>
    private static void ExecuteActionReport(string selectionDisplayname, string actionType, string outputPath,
                                            bool done)
    {
      try
      {
        var key = $"{selectionDisplayname} > {actionType} > {Path.GetFileName(outputPath)}";
        lock (_executeActionListLock)
        {
          // Entferne bereits erledigte Aufgaben
          var keys = _executeActionList.Keys.ToArray();
          foreach (var k in keys)
            if (_executeActionList[k])
              _executeActionList.Remove(k);

          // Status aktualisieren
          if (_executeActionList.ContainsKey(key))
            _executeActionList[key] = done;
          else
            _executeActionList.Add(key, done);

          // Liste ausgeben
          System.Console.ForegroundColor = ConsoleColor.Gray;
          ConsoleHelper.PrintHeader();
          System.Console.WriteLine("..:: CURRENT ACTIONS ::..");
          foreach (var t in _executeActionList)
          {
            System.Console.ForegroundColor = t.Value ? ConsoleColor.Green : ConsoleColor.Yellow;
            System.Console.WriteLine($"{t.Key} ... {(t.Value ? "done" : "running")}");
          }

          System.Console.ForegroundColor = ConsoleColor.Gray;
        }
      }
      catch (Exception ex)
      {
        LogError(ex);
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

      if (queries?.Items == null)
        return res;

      foreach (var item in queries.Items)
        try
        {
          switch (item)
          {
            case query q:
              if (string.IsNullOrWhiteSpace(q.parent))
                GenerateSelections_SingleQuery(q, ref res, source.SelectAll);
              else
                foreach (var selection in res[q.parent])
                  GenerateSelections_SingleQuery(q, ref res, selection, true);
              break;
            case queryBuilder b:
              if (string.IsNullOrWhiteSpace(b.parent))
                GenerateSelections_QueryBuilder(b, ref res, source.SelectAll);
              else
                foreach (var selection in res[b.parent])
                  GenerateSelections_QueryBuilder(b, ref res, selection, true);
              break;
            case queryGroup g:
              if (string.IsNullOrWhiteSpace(g.parent))
                GenerateSelections_QueryGroup(g, ref res, source.SelectAll);
              else
                foreach (var selection in res[g.parent])
                  GenerateSelections_QueryGroup(g, ref res, selection, true);
              break;
          }
        }
        catch (Exception ex)
        {
          LogError(ex);
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
      catch (Exception ex)
      {
        LogError(ex);
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
    /// <param name="useSelectionDisplaynameAsPrefix">Stellt dem Namen des aktuell erzeugten Query den ParentDisplayname voran.</param>
    private static void GenerateSelections_QueryBuilder(queryBuilder queryBuilder,
                                                        ref Dictionary<string, Selection[]> res, Selection all,
                                                        bool useSelectionDisplaynameAsPrefix = false)
    {
      var key = queryBuilder.name ?? string.Empty;
      if (key == "*" || key == "+")
        return;
      if (useSelectionDisplaynameAsPrefix)
        key = $"{all.Displayname}_{key}";
      foreach (var v in queryBuilder.value)
      {
        var gname = (key + v).Replace("\"", "");
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
    /// <param name="useSelectionDisplaynameAsPrefix">Stellt dem Namen des aktuell erzeugten Query den ParentDisplayname voran.</param>
    private static void GenerateSelections_QueryGroup(queryGroup queryGroup, ref Dictionary<string, Selection[]> res,
                                                      Selection all, bool useSelectionDisplaynameAsPrefix = false)
    {
      var key = queryGroup.name ?? string.Empty;
      if (key == "" || key == "*" || key == "+")
        return;
      if (useSelectionDisplaynameAsPrefix)
        key = $"{all.Displayname}_{key}";
      if (res.ContainsKey(key))
        return;

      var prefix = string.IsNullOrEmpty(queryGroup.prefix) ? string.Empty : queryGroup.prefix;

      // Erzeuge erste Abfrage
      var qs = new List<query>(queryGroup.query);
      var selection = GenerateSelections_Compile(all, $"{prefix}{qs[0].Text.CleanXmlValue()}", qs[0].name).First()
                                                                                                          .CorporaAndDocumentGuids
                                                                                                          .ToDictionary(x => x.Key,
                                                                                                                        x =>
                                                                                                                          new
                                                                                                                            HashSet
                                                                                                                            <Guid
                                                                                                                            >(x
                                                                                                                               .Value));
      qs.RemoveAt(0); // Entferne erste Abfrage aus der Liste

      // Führe alle Folgeabfragen aus.
      foreach (var query in qs)
      {
        var temp = GenerateSelections_Compile(all, $"{prefix}{query.Text.CleanXmlValue()}", "").First()
                                                                                               .CorporaAndDocumentGuids
                                                                                               .ToDictionary(x => x.Key,
                                                                                                             x =>
                                                                                                               new
                                                                                                                 HashSet
                                                                                                                 <Guid
                                                                                                                 >(x
                                                                                                                    .Value));
        switch (queryGroup.@operator)
        {
          default:
          // ReSharper disable once RedundantCaseLabel
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
    /// <param name="useSelectionDisplaynameAsPrefix">Stellt dem Namen des aktuell erzeugten Query den ParentDisplayname voran.</param>
    private static void GenerateSelections_SingleQuery(query query, ref Dictionary<string, Selection[]> res,
                                                       Selection all, bool useSelectionDisplaynameAsPrefix = false)
    {
      var key = query.name ?? string.Empty;
      if (key == "" || key == "*" || key == "+")
        return;
      if (useSelectionDisplaynameAsPrefix)
        key = $"{all.Displayname}_{key}";
      if (res.ContainsKey(key))
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
        LogError(ex);
      }

      return script;
    }

    private static void LogError(Exception ex, string additionalLine = null)
    {
      try
      {
        File.AppendAllLines(_errorLog,
                            additionalLine == null
                              ? new[] {ex.Message, ex.StackTrace, "---"}
                              : new[] {additionalLine, ex.Message, ex.StackTrace, "---"});
      }
      catch
      {
        // ignore
      }
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
      if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
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
      if (sources.annotate().Any())
      {
        var scrapers = Configuration.AddonScrapers.GetDictionary();
        var taggers = Configuration.AddonTaggers.GetDictionary();

        foreach (var annotate in sources.annotate())
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
          catch (Exception ex)
          {
            LogError(ex);
          }
      }

      // Wenn Import-Quellen vorhanden sind, dann lese diese ein.
      if (sources.import().Any())
      {
        var importers = Configuration.AddonImporters.GetDictionary();
        foreach (var import in sources.import())
          try
          {
            if (!importers.ContainsKey(import.type))
              continue;

            foreach (var corpus in importers[import.type].Execute(SearchFiles(import.Items, ref deletePaths)))
              res.Add(corpus);
          }
          catch (Exception ex)
          {
            LogError(ex);
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
        catch (Exception ex)
        {
          LogError(ex);
        }

      return res;
    }
  }
}