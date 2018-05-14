using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;
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
    public static bool IsXmlScript(string path)
    {
      try
      {
        var lines = File.ReadAllLines(path, Configuration.Encoding);
        var max = lines.Length < 5 ? lines.Length : 5;
        for (var i = 0; i < max; i++)
        {
          if (lines[i].ToLower().Contains("<cescript>"))
            return true;
        }

        return false;
      }
      catch
      {
        return false;
      }
    }

    public static void Process(string path, Dictionary<string, AbstractAction> actions, Dictionary<string, AbstractTableWriter> formats)
    {
      cescript script = null;
      var scriptFilename = Path.GetFileNameWithoutExtension(path);
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

      if (script == null)
      {
        System.Console.WriteLine("XML Parser Error 001");
        return;
      }

      Parallel.ForEach(script.sessions, Configuration.ParallelOptions, session =>
      {
        try
        {
          var source = ReadSources(session.sources);
          var selections = GenerateSelections(source, session.queries);

          Parallel.ForEach(session.tasks, Configuration.ParallelOptions, task =>
          {
            try
            {
              if (!actions.ContainsKey(task.type))
                return;
              var action = actions[task.type];

              if (task.query == "*")
                Parallel.ForEach(selections, Configuration.ParallelOptions, selection =>
                {
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
      });
    }

    private static void ExecuteTask(AbstractAction action, task task, Dictionary<string, AbstractTableWriter> formats,
      Selection selection, string scriptFilename)
    {
      if (task.type == "query" || task.type == "convert")
      {
        var exporters = Configuration.AddonExporters.GetDictionary();
        if (!exporters.ContainsKey(task.output.format))
          return;

        exporters[task.output.format].Export(selection, OutputPathBuilder(task.output.Value, scriptFilename, selection.Displayname, task.type));
      }
      else
      {
        var formatKey = task.output.format.StartsWith("F:") ? task.output.format : $"F:{task.output.format}";
        if (!formats.ContainsKey(formatKey))
          return;

        var format = formats[formatKey];
        using (var fs = new FileStream(OutputPathBuilder(task.output.Value, scriptFilename, selection.Displayname, task.type), FileMode.Create, FileAccess.Write))
        {
          format.OutputStream = fs;
          ConsoleConfiguration.Writer = format;
          action.Execute(selection, task.arguments);
        }
      }
    }

    private static Dictionary<string, Selection[]> GenerateSelections(Project source, queries queries)
    {
      var res = new Dictionary<string, Selection[]> { { "", new[] { source.ToSelection() } } };

      foreach (var item in queries.Items)
      {
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
      }

      return res;
    }

    private static void GenerateSelections_SingleQuery(query query1, ref Dictionary<string, Selection[]> res, Selection all)
    {
      var key = query1.name ?? string.Empty;
      if (key == "" || key == "*" || res.ContainsKey(key))
        return;
      res.Add(key, GenerateSelections_Compile(all, query1.Text.CleanXmlValue(), query1.name));
    }

    private static void GenerateSelections_QueryBuilder(queryBuilder queryBuilder, ref Dictionary<string, Selection[]> res, Selection all)
    {
      var key = queryBuilder.name ?? string.Empty;
      if (key == "" || key == "*" || queryBuilder.value == null)
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

    private static void GenerateSelections_QueryGroup(queryGroup queryGroup, ref Dictionary<string, Selection[]> res, Selection all)
    {
      var key = queryGroup.name ?? string.Empty;
      if (key == "" || key == "*" || res.ContainsKey(key))
        return;

      var qs = new List<query>(queryGroup.query);
      var selection = GenerateSelections_Compile(all, qs[0].Text.CleanXmlValue(), qs[0].name).First()
        .CorporaAndDocumentGuids.ToDictionary(x => x.Key, x => new HashSet<Guid>(x.Value));
      qs.RemoveAt(0);

      foreach (var query in qs)
      {
        var temp = GenerateSelections_Compile(all.CreateTemporary(selection), query.Text.CleanXmlValue(), "").First()
          .CorporaAndDocumentGuids.ToDictionary(x => x.Key, x => new HashSet<Guid>(x.Value));
        switch (queryGroup.@operator)
        {
          default:
          case "and":
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
          case "or":
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

      res.Add(key, new[] { all.Create(selection, key) });
    }

    private static Selection[] GenerateSelections_Compile(Selection selection, string query, string key)
    {
      try
      {
        var filterQuery = QueryParser.Parse(query.CleanXmlValue());
        if (!(filterQuery is FilterQueryUnsupportedParserFeature))
          return new[] { selection.Create(new[] { filterQuery }, key) };

        var q = (FilterQueryUnsupportedParserFeature)filterQuery;
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

    private static Selection[] GenerateSelections_MetaSplit(Selection selection, FilterQueryUnsupportedParserFeature q, IEnumerable<object> values)
    {
      var vs = values?.ToArray();
      if (vs?.Length != 1)
        return null;

      var block = AutoSplitBlockHelper.RunAutoSplit(selection, q, vs);
      return block.GetSelectionClusters().ToArray();
    }

    private static Selection[] GenerateSelections_CorporaSplit(Selection selection)
    {
      return
         (from csel in selection.CorporaGuids
          let corpus = selection.GetCorpus(csel)
          let dsels = new HashSet<Guid>(corpus.DocumentGuids)
          select selection.Create(new Dictionary<Guid, HashSet<Guid>> { { csel, dsels } }, corpus.CorpusDisplayname))
        .ToArray();
    }

    private static Selection[] GenerateSelections_RandomSplit(Selection selection, IEnumerable<object> values)
    {
      var block = selection.CreateBlock<RandomSelectionBlock>();
      block.DocumentCount = int.Parse(values.First().ToString());
      block.Calculate();
      return new[] { block.RandomSelection, block.RandomInvertSelection };
    }

    private static Project ReadSources(sources sources)
    {
      var res = CorpusExplorerEcosystem.InitializeMinimal();

      if (sources.annotate != null)
      {
        var scrapers = Configuration.AddonScrapers.GetDictionary();
        var taggers = Configuration.AddonTaggers.GetDictionary();

        foreach (var annotate in sources.annotate)
        {
          try
          {
            if (!scrapers.ContainsKey(annotate.type))
              continue;
            if (!taggers.ContainsKey(annotate.tagger))
              continue;

            var scraper = scrapers[annotate.type];
            scraper.Input.Enqueue(SearchFiles(annotate.Items));
            scraper.Execute();
            var cleaner1 = new StandardCleanup { Input = scraper.Output };
            cleaner1.Execute();
            var cleaner2 = new RegexXmlMarkupCleanup { Input = cleaner1.Output };
            cleaner2.Execute();

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
      }

      // ReSharper disable once InvertIf
      if (sources.import != null)
      {
        var importers = Configuration.AddonImporters.GetDictionary();
        foreach (var import in sources.import)
        {
          try
          {
            if (!importers.ContainsKey(import.type))
              continue;

            foreach (var corpus in importers[import.type].Execute(SearchFiles(import.Items)))
              res.Add(corpus);
          }
          catch
          {
            //ignore
          }
        }
      }

      return res;
    }

    private static IEnumerable<string> SearchFiles(object[] annotateItems)
    {
      var res = new List<string>();
      foreach (var item in annotateItems)
      {
        try
        {
          switch (item)
          {
            case file i:
              res.Add(i.Value);
              break;
            case directory i:
              res.AddRange(Directory.GetFiles(i.Value, i.filter, SearchOption.TopDirectoryOnly));
              break;
          }
        }
        catch
        {
          // ignore
        }
      }

      return res;
    }

    private static string OutputPathBuilder(string path, string scriptFilename, string selectionName, string action)
    {
      var res = path.Replace("{all}", "{script}_{selection}_{action}").Replace("{script}", scriptFilename).Replace("{selection}", selectionName == "*" ? "ALL" : selectionName).Replace("{action}", action).EnsureFileName();
      var dir = Path.GetDirectoryName(res);
      if (dir != null && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      return res;
    }
  }
}