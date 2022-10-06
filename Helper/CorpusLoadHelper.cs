using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
               : path.StartsWith("import#")
                 ? LoadCorpusImport(path)
                 : null;
    }

    private static AbstractCorpusAdapter LoadCorpusAnnotate(string path)
    {
      // Bsp.: annotate#BundestagPlenarprotokolleScraper#[TAGGER]#[LANGUAGE]#[DIRECTORY]
      var split = path.Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries).ToList();
      if (split.Count != 5)
        return null;

      split.RemoveAt(0); // entfernt annotate#

      var scraper = Configuration.AddonScrapers.GetReflectedType(split[0], "Scraper");
      if (scraper == null)
        return null;
      split.RemoveAt(0); // entfernt [SCRAPER]

      // Cleaner bereinigen Meta-/Textdaten
      var cleaner = new StandardCleanup();

      // Tagger annotieren Textdaten
      var tagger = Configuration.AddonTaggers.GetReflectedType(split[0], "Tagger");
      if (tagger == null)
        return null;
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
      // Bsp.: import#ImporterCec6#[FILES]
      var split = path.Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length != 3)
        return null;

      var importer = Configuration.AddonImporters.GetReflectedType(split[1], "Importer");
      if (importer == null)
        return null;

      var files = DetectFileOrDirectoryPaths(split[2]);

      var res = importer.Execute(files).ToArray();
      if (res.Length == 1)
        return res[0];

      // Falls mehrere Korpora importiert werden, füge diese zusammen
      var merger = new CorpusMerger {CorpusBuilder = new CorpusBuilderWriteDirect()};
      foreach (var x in res)
        if (x != null)
          merger.Input(x);
      merger.Execute();
      return merger.Output.FirstOrDefault();
    }

    private static char[] _separator = {';', '\''};

    private static List<string> DetectFileOrDirectoryPaths(string fileOrDirectory)
    {
      var tmp = fileOrDirectory.Split(_separator, StringSplitOptions.RemoveEmptyEntries);
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