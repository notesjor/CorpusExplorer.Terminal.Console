using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class CorpusLoadHelper
  {
    public static AbstractCorpusAdapter LoadCorpus(string path)
    {
      return path.StartsWith("annotate#")
               ? LoadCorpusAnnotate(path)
               : (path.StartsWith("import#")
                    ? LoadCorpusImport(path)
                    : null);
    }

    private static AbstractCorpusAdapter LoadCorpusAnnotate(string path)
    {
      // Scraper extrahieren Meta-/Textdaten
      var scrapers = Configuration.AddonScrapers.GetDictionary();
      var split = path.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries).ToList();
      if (split.Count != 5)
        return null;

      split.RemoveAt(0); // entfernt annotate#
      if (!scrapers.ContainsKey(split[0]))
        return null;

      var scraper = scrapers[split[0]];
      // Cleaner bereinigen Meta-/Textdaten
      var cleaner = new StandardCleanup();
      split.RemoveAt(0); // entfernt [SCRAPER]

      // Tagger annotieren Textdaten
      var taggers = Configuration.AddonTaggers.GetDictionary();
      if (!taggers.ContainsKey(split[0]))
        return null;

      var tagger = taggers[split[0]];
      split.RemoveAt(0); // entfernt [TAGGER]
      tagger.LanguageSelected = split[0];
      split.RemoveAt(0); // entfernt [LANGUAGE]
      var files = Directory.GetFiles(split[0].Replace("\"", ""), "*.*", SearchOption.TopDirectoryOnly);

      // Nachdem alle Informationen vorliegen, arbeite die Dateien ab.
      scraper.Input.Enqueue(files);
      scraper.Execute();
      cleaner.Input.Enqueue(scraper.Output);
      cleaner.Execute();
      tagger.Input.Enqueue(cleaner.Output);
      tagger.Execute();

      return tagger.Output.FirstOrDefault();
    }

    private static AbstractCorpusAdapter LoadCorpusImport(string path)
    {
      // Importer laden bestehende Korpora
      var importers = Configuration.AddonImporters.GetDictionary();
      var split = path.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length != 3)
        return null;

      if (!importers.ContainsKey(split[1]))
        return null;
      var importer = importers[split[1]];

      var files = DetectFileOrDirectoryPaths(split[2]);

      var res = importer.Execute(files).ToArray();
      if (res.Length == 1)
        return res[0];

      // Falls mehrere Korpora importiert werden, füge diese zusammen
      var merger = new CorpusMerger { CorpusBuilder = new CorpusBuilderWriteDirect() };
      foreach (var x in res)
        if (x != null)
          merger.Input(x);
      merger.Execute();
      return merger.Output.FirstOrDefault();
    }

    private static List<string> DetectFileOrDirectoryPaths(string fileOrDirectory)
    {
      var tmp = fileOrDirectory.Split(new[] { "|", "\"" }, StringSplitOptions.RemoveEmptyEntries);
      var files = new List<string>();
      foreach (var x in tmp)
        if (x.IsDirectory())
          files.AddRange(Directory.GetFiles(x, "*.*"));
        else
          files.Add(x);

      return files;
    }
  }
}
