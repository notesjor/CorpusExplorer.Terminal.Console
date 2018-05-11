using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

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

              if (task.query != "*")
                Parallel.ForEach(selections, selection =>
                {
                  try
                  {
                    ExecuteTask(action, task, formats, selection.Value, scriptFilename);
                  }
                  catch
                  {
                    // ignore
                  }
                });
              else
              {
                if (!selections.ContainsKey(task.query ?? string.Empty))
                  return;

                ExecuteTask(action, task, formats, selections[task.query ?? string.Empty], scriptFilename);
              }
            }
            catch
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

    private static object _excecuteLock = new object();

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
        lock (_excecuteLock)
        {
          if (!formats.ContainsKey(task.output.format))
            return;

          var format = formats[task.output.format];
          using (var fs = new FileStream(OutputPathBuilder(task.output.Value, scriptFilename, selection.Displayname, task.type), FileMode.Create, FileAccess.Write))
          {
            format.OutputStream = fs;
            ConsoleConfiguration.Writer = format;
            action.Execute(selection, task.arguments);
          }
        }
      }
    }

    private static Dictionary<string, Selection> GenerateSelections(AbstractCorpusAdapter source, queries queries)
    {
      var all = source.ToSelection();
      all.Displayname = "";

      var res = new Dictionary<string, Selection>();
      foreach (var q in queries.query)
      {
        var key = q.name ?? string.Empty;
        if (key != "*" && !res.ContainsKey(key))
          res.Add(key, null);
      }

      var keys = res.Keys.ToArray();
      foreach (var key in keys)
      {
        try
        {
          var qs = (from x in queries.query where x.name == key select QueryParser.Parse(string.Join(" ", x.Text)))
            .Where(x => x != null);
          res[key] = all.Create(qs, key);
        }
        catch
        {
          // ignore
        }
      }

      if (!res.ContainsKey(""))
        res.Add("", all);

      return res;
    }

    private static AbstractCorpusAdapter ReadSources(sources sources)
    {
      var res = new List<AbstractCorpusAdapter>();

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

            res.AddRange(tagger.Output);
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

            res.AddRange(importers[import.type].Execute(SearchFiles(import.Items)));
          }
          catch
          {
            //ignore
          }
        }
      }

      return CorpusMerger.Merge(res, new CorpusBuilderWriteDirect());
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
            case string i:
              res.Add(i);
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
      var res = path.Replace("{script}", scriptFilename).Replace("{selection}", selectionName).Replace("{action}", action);
      var dir = Path.GetDirectoryName(res);
      if (dir != null && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      return res;
    }
  }
}