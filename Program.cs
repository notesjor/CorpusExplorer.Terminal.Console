using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CorpusExplorer.Terminal.Console.Action;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console
{
  public class Program
  {
    private static readonly AbstractAction[] _actions =
    {
      new Frequency1Action(),
      new Frequency2Action(),
      new Frequency3Action(),
      new CooccurrenceAction(),
      new MetaAction(),
      new CrossFrequencyAction(),
      new OutputAction(),
      new FilterAction(),
      new NGramAction(),
      new VocabularyComplexityAction(),
      new ReadingEaseAction(),
      new LayerNamesAction(),
      new MetaCategoriesAction(),
      new TokenCountAction(),
      new DocumentCountAction(),
      new SentenceCountAction()
    };

    private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        var dll = args.Name.Substring(0, args.Name.IndexOf(",")) + ".dll";
        var path = Path.Combine(
          Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
          @"CorpusExplorer\App",
          dll);
        return !File.Exists(path) ? null : Assembly.LoadFrom(path);
      }
      catch
      {
        return null;
      }
    }

    private static void Execute(string[] args)
    {
      CorpusExplorerEcosystem.Initialize(new CacheStrategyDisableCaching());

      if (args == null || args.Length == 0)
      {
        PrintHelp();
        return;
      }
      
      var corpus = LoadCorpus(args[0]);
      var selection = corpus?.ToSelection();
      if (selection == null || selection.CountToken == 0)
        return;

      System.Console.OutputEncoding = Encoding.UTF8;

      foreach (var action in _actions)
      {
        if (!action.Match(args[1]))
          continue;

        var temp = args.ToList();
        temp.RemoveAt(0); // CorpusFile (no longer needed)
        temp.RemoveAt(0); // Action (no longer needed)
        action.Execute(selection, temp);
        return;
      }
    }

    private static AbstractCorpusAdapter LoadCorpus(string path)
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
      var split = path.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries).ToList();
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
      var split = path.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries).ToList();
      if (split.Count < 3)
        return null;

      split.RemoveAt(0); // entfernt import#
      if (!importers.ContainsKey(split[0]))
        return null;

      split.RemoveAt(0); // entfernt [IMPORTER]
      for (var i = 0; i < split.Count; i++)
        split[i] = split[i].Replace("\"", "");

      var res = importers[split[1]].Execute(split).ToArray();
      if (res.Length == 1)
        return res[0];

      // Falls mehrere Korpora importiert werden, füge diese zusammen
      var merger = new CorpusMerger {CorpusBuilder = new CorpusBuilderSingleFile()};
      foreach (var x in res)
        if (x != null)
          merger.Input(x);
      merger.Execute();
      return merger.Output.FirstOrDefault();
    }

    private static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      Execute(args);
    }

    private static void PrintHelp()
    {
      System.Console.WriteLine("CorpusExplorer v2.0");
      System.Console.WriteLine("Copyright 2013-2017 by Jan Oliver Rüdiger");
      System.Console.WriteLine();
      System.Console.WriteLine("ceRport.exe [INPUT] [TASK]");
      System.Console.WriteLine();
      System.Console.WriteLine("[INPUT]");
      System.Console.WriteLine("Available importer:");

      var importer = Configuration.AddonImporters.GetDictionary();
      foreach (var x in importer)
      {
        System.Console.WriteLine($"ceRport.exe import#{x.Key}#[FILES] [TASK]");
      }
      var scraper = Configuration.AddonScrapers.GetDictionary();
      System.Console.WriteLine("Available scraper:");
      foreach (var x in scraper)
      {
        System.Console.WriteLine($"ceRport.exe annotate#{x.Key}#[TAGGER]#[LANGUAGE]#[DIRECTORY] [TASK]");
      }
      var tagger = Configuration.AddonTaggers.GetDictionary();
      System.Console.WriteLine("Available tagger & languages:");
      foreach (var x in tagger)
      {
        System.Console.WriteLine($"ceRport.exe annotate#[SCRAPER]#{x.Key}#[LANGUAGE]#[DIRECTORY] [TASK]");
        System.Console.WriteLine($"supported languages: {string.Join(", ", x.Value.LanguagesAvailabel)}");
      }
      var exporter = Configuration.AddonExporters.GetDictionary();
      System.Console.WriteLine("Available exporters (convert / query):");
      foreach (var x in exporter)
      {
        System.Console.WriteLine($"ceRport.exe [INPUT] convert {x.Key}#[FILE]");
        System.Console.WriteLine($"ceRport.exe [INPUT] query [QUERY] {x.Key}#[FILE]");
      }
    }
  }
}