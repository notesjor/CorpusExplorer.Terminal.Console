using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Cache;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Terminal.Console.Action;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer;
using CorpusExplorer.Terminal.Console.Writer.Abstract;
using CorpusExplorer.Terminal.Console.Xml.Processor;

namespace CorpusExplorer.Terminal.Console
{
  public class Program
  {
    private static readonly string _appPath =
      Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cec.exe");

    private static readonly Dictionary<string, AbstractAction> _actions = new AbstractAction[]{
      new BasicInformationAction(),
      new LayerNamesAction(),
      new MetaCategoriesAction(),

      new DocumentCountAction(),
      new SentenceCountAction(),
      new TokenCountAction(),
      new LayerValuesAction(),
      new TypeCountAction(),

      new Frequency1Action(),
      new Frequency2Action(),
      new Frequency3Action(),
      new NGramAction(),
      new CrossFrequencyAction(),
      new CooccurrenceAction(),
      new MetaAction(),
      new MetaDocumentAction(),

      new VocabularyComplexityAction(),
      new ReadingEaseAction(),

      new KwicAnyFilterAction(),
      new KwicAllInDocumentFilterAction(),
      new KwicAllInSentenceFilterAction(),
      new KwicExactPhraseFilterAction(),

      new OutputAction(),
      new FilterAction(),
    }.ToDictionary(x => x.Action, x => x);

    private static Dictionary<string, AbstractTableWriter> _formats = new Dictionary<string, AbstractTableWriter>
    {
      {"F:TSV", new TsvTableWriter()},
      {"F:CSV", new CsvTableWriter()},
      {"F:XML", new XmlTableWriter()},
      {"F:SQL", new SqlTableWriter()},
      {"F:JSON", new JsonTableWriter()},
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

      if (args[0].StartsWith("F:"))
      {
        ConsoleConfiguration.Writer = _formats.ContainsKey(args[0]) ? _formats[args[0]] : _formats.First().Value;
        
        var list = new List<string>(args);
        list.RemoveAt(0);
        args = list.ToArray();
      }

      if (args[0].StartsWith("FILE:"))
        ExecuteSkript(args);
      else if (args[0].StartsWith("DEBUG:"))
        DebugSkript(args);
      else if (args[0].ToLowerInvariant() == "shell")
        ExecuteShell();
      else if (args.Length == 1 && File.Exists(args[0]))
      {
        args[0] = "FILE:" + args[0];
        ExecuteSkript(args);
      }
      else
        ExecuteDirect(args);

      ConsoleConfiguration.Dispose();
    }

    private static void ExecuteShell()
    {
      System.Console.WriteLine();
      System.Console.WriteLine("CorpusExplorer v2.0");
      System.Console.WriteLine("Copyright 2013-2017 by Jan Oliver Rüdiger");
      System.Console.WriteLine();
      System.Console.WriteLine("help - to display command help");
      System.Console.WriteLine("FILE:[FILE] - to execute script");
      System.Console.WriteLine("SAVE:[FILE] - to save command history as script");
      System.Console.WriteLine("quit - to exit shell mode");
      System.Console.WriteLine();

      var history = new List<string>();
      while (true)
      {
        System.Console.Write("cec.exe ");
        var command = System.Console.ReadLine();
        switch (command)
        {
          case "quit":
          case "exit":
            return;
          case "help":
            PrintHelp();
            break;
          default:
            if (command.StartsWith("SAVE:"))
              File.WriteAllLines(command.Replace("SAVE:", ""), history.ToArray());
            else
            {
              history.Add(command);
              StartProcessCec(command);
            }

            break;
        }
      }
    }

    private static void ExecuteSkript(string[] args)
    {
      var path = args[0].Replace("FILE:", "").Replace("\"", "");
      if (ProcessXmlScript(path))
        return;

      var lines = File.ReadAllLines(path, Configuration.Encoding);

      foreach (var line in lines)
        StartProcessCec(line);
    }

    private static void DebugSkript(string[] args)
    {
      var path = args[0].Replace("DEBUG:", "").Replace("\"", "");
      if (ProcessXmlScript(path))
        return;

      var lines = File.ReadAllLines(path, Configuration.Encoding);

      System.Console.WriteLine($"execute script: {path}");
      for (var i = 0; i < lines.Length; i++)
      {
        try
        {
          var line = lines[i];
          System.Console.Write($"[{i + 1:D3}/{lines.Length:D3}] {line}");
          StartProcessCec(line);
          System.Console.WriteLine("...ok!");
        }
        catch (Exception ex)
        {
          System.Console.WriteLine(ex.Message);
          System.Console.WriteLine(ex.StackTrace);
        }
      }
    }

    private static bool ProcessXmlScript(string path)
    {
      if (!XmlScriptProcessor.IsXmlScript(path))
        return false;

      try
      {
        XmlScriptProcessor.Process(path, _actions, _formats);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static void StartProcessCec(string argument)
    {
      if(string.IsNullOrEmpty(argument))
        return;
      if(argument.StartsWith("#"))
      {
        System.Console.WriteLine(argument);
        return;
      }

      if (argument.Contains(" > "))
      {
        var split = argument.Split(new[] {" > "}, StringSplitOptions.None).ToList();
        argument = split[0];

        var process = Process.Start(new ProcessStartInfo
        {
          Arguments = !argument.StartsWith("F:") ? $"{ConsoleConfiguration.Writer.TableWriterTag} {argument}" : argument,
          CreateNoWindow = true,
          FileName = _appPath,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardOutput = true,
          StandardOutputEncoding = Configuration.Encoding,
          UseShellExecute = false
        });
        var res = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        File.WriteAllText(split[1], res, Configuration.Encoding);
      }
      else
      {
        var process = Process.Start(new ProcessStartInfo
        {
          Arguments = !argument.StartsWith("F:") ? $"{ConsoleConfiguration.Writer.TableWriterTag} {argument}" : argument,
          CreateNoWindow = true,
          FileName = _appPath,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardOutput = true,
          StandardOutputEncoding = Configuration.Encoding,
          UseShellExecute = false
        });
        var res = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        System.Console.Out.Write(res);
      }      
    }

    private static void ExecuteDirect(string[] args)
    {
      var corpus = LoadCorpus(args[0]);
      var selection = corpus?.ToSelection();
      if (selection == null || selection.CountToken == 0)
        return;

      var task = args[1].ToLowerInvariant();

      if (!_actions.ContainsKey(task))
        return;
      
      System.Console.OutputEncoding = Configuration.Encoding;
      var temp = args.ToList();
      temp.RemoveAt(0); // CorpusFile (no longer needed)
      temp.RemoveAt(0); // Action (no longer needed)
      _actions[task].Execute(selection, temp.ToArray());
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
      {
        if (x.IsDirectory())
          files.AddRange(Directory.GetFiles(x, "*.*"));
        else
          files.Add(x);
      }

      return files;
    }

    private static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      Execute(args);
    }

    private static void PrintHelp()
    {
      System.Console.WriteLine();
      System.Console.WriteLine("CorpusExplorer v2.0");
      System.Console.WriteLine("Copyright 2013-2017 by Jan Oliver Rüdiger");
      System.Console.WriteLine();
      System.Console.WriteLine("Syntax for annotation/conversion:");
      System.Console.WriteLine("cec.exe [INPUT] [OUTPUT]");
      System.Console.WriteLine("Syntax for filtering:");
      System.Console.WriteLine("cec.exe [INPUT] [QUERY] [OUTPUT]");
      System.Console.WriteLine("Syntax for analytics (writes TSV-output to stdout):");
      System.Console.WriteLine("cec.exe [F:FORMAT] [INPUT] [TASK]");
      System.Console.WriteLine("Syntax for scripting:");
      System.Console.WriteLine("cec.exe FILE:[PATH]");
      System.Console.WriteLine("More detailed scripting errors:");
      System.Console.WriteLine("cec.exe DEBUG:[PATH]");
      System.Console.WriteLine("To start interactive shell mode");
      System.Console.WriteLine("cec.exe SHELL");
      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [INPUT] --- :>");
      System.Console.WriteLine();
      System.Console.WriteLine("Import corpus material - direct [INPUT]:");

      var importer = Configuration.AddonImporters.GetDictionary();
      foreach (var x in importer)
      {
        System.Console.WriteLine($"[INPUT] = import#{x.Key}#[FILES]");
      }

      System.Console.WriteLine("Note: [FILES] = separate files with & - merges all files before processing");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus1.cec5&C:\\mycorpus2.cec5 convert ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();

      var scraper = Configuration.AddonScrapers.GetDictionary();
      System.Console.WriteLine();
      System.Console.WriteLine("Annotate raw text - indirect [INPUT]:");
      foreach (var x in scraper)
      {
        System.Console.WriteLine($"[INPUT] = annotate#{x.Key}#[TAGGER]#[LANGUAGE]#[DIRECTORY]");
      }
      System.Console.WriteLine("Note: [DIRECTORY] = any directory you like - all files will be processed");
      var tagger = Configuration.AddonTaggers.GetDictionary();
      System.Console.WriteLine("[TAGGER] & [LANGUAGE]:");
      foreach (var x in tagger)
      {
        System.Console.Write($"[TAGGER] = {x.Key} ");
        System.Console.WriteLine($"([LANGUAGE] = {string.Join(", ", x.Value.LanguagesAvailabel)})");
      }
      System.Console.WriteLine("Example: cec.exe annotate#DpxcScraper#SimpleTreeTagger#Deutsch#C:\\dpxc\\ convert ExporterCec6#C:\\mycorpus.cec6");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [OUTPUT] --- :>");
      System.Console.WriteLine();
      var exporter = Configuration.AddonExporters.GetDictionary();
      System.Console.WriteLine("Direct [OUTPUT]:");
      foreach (var x in exporter)
      {
        System.Console.WriteLine($"[OUTPUT] = convert {x.Key}#[FILE]");
      }
      System.Console.WriteLine("Note: [FILE] = any file you like to store the output");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 convert ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();
      System.Console.WriteLine("Filtered [OUTPUT]:");
      foreach (var x in exporter)
      {
        System.Console.WriteLine($"[OUTPUT] = query [QUERY] {x.Key}#[FILE]");
      }
      System.Console.WriteLine("Note: [FILE] = any file you like to store the output");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query  ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();
      System.Console.WriteLine("[QUERY]:");
      System.Console.WriteLine("A preceding ! inverts the entiere query");
      System.Console.WriteLine("First character:");
      System.Console.WriteLine("M = Metadata -OR- T = (Full)Text -OR- X = Extended Features");
      System.Console.WriteLine("Second character [OPERATOR] (if you choose M):");
      System.Console.WriteLine("? = regEx | : = contains (case sensitive) | . = contains (not case sensitive)");
      System.Console.WriteLine("= = match exact (case sensitive) | - = match exact (not case sensitive) | ! = is empty");
      System.Console.WriteLine("( = starts with (case sensitive) | ) = ends with (case sensitive)");
      System.Console.WriteLine("Second character [OPERATOR] (if you choose T):");
      System.Console.WriteLine("~ = any match | - = all in one document | = = all in one sentence | § = exact phrase");
      System.Console.WriteLine("Second character [OPERATOR] (if you choose X):");
      System.Console.WriteLine("R = random selection | S = auto split by meta-data");
      System.Console.WriteLine("If you have chosen M - enter the name of the meta category (see [TASK] = meta-categories)");
      System.Console.WriteLine("If you have chosen XS - enter the name of the meta category (see [TASK] = meta-categories)");
      System.Console.WriteLine("If you have chosen T - enter the layer name (see [TASK] = layer-names)");
      System.Console.WriteLine("Enter the separator :: followed by the query");
      System.Console.WriteLine("Example (query only): !M:Author::Jan - Finds all documents where \"Jan\" isn't an author");
      System.Console.WriteLine("Example (in action): cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query !M:Author::Jan ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("OR");
      System.Console.WriteLine("Example (query only): T§Wort::OpenSource;Software - Finds all documents with the exact phrase \"OpensSource Software\"");
      System.Console.WriteLine("Example (in action): cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query T§Wort::OpenSource;Software ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("Note 1: If you use several words in a T-query, then separate them with ;");
      System.Console.WriteLine("Note 2: You can also use a query file (*.ceusd) - use the FILE: prefix");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query FILE:C:\\query.ceusd ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("Note 3: You cann't load a X (XR or XS) query from file");
      System.Console.WriteLine("If you use XR for random selection you need to specify the document count");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query XR::100 ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("Note 4: XR will generate two outputs - the regular and the inverted output.");
      System.Console.WriteLine("If you use XS you must specify the meta data type - TEXT, INT, FLOAT or DATE");
      System.Console.WriteLine("Note 5: XS will generate multiple outputs - based on clusters.");
      System.Console.WriteLine("TEXT generates for every entry a separate snapshot");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query XSAuthor::TEXT ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("INT / FLOAT you need to set up a [CLUSTERSIZE]");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query XSYear::INT;10 ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("DATE;C;[CLUSTERSIZE] - generates [CLUSTERSIZE] clusters.");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query XSDate::DATE;C;10 ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("DATE;CEN = Century-Cluster / DATE;DEC = Decate-Cluster");
      System.Console.WriteLine("DATE;Y = Year-Cluster / DATE;YM = Year/Month-Cluster / DATE;YMD = Year/Month/Day-Cluster");
      System.Console.WriteLine("DATE;YMDH = Year/Month/Day/Hour-Cluster / DATE;YMDHM = Year/Month/Day/Hour/Minute-Cluster / ALL = Every-Time-Cluster");      
      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query XSDate::DATE;YMD ExporterCec6#C:\\mycorpus.cec6");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [TASK] --- :>");
      System.Console.WriteLine();

      foreach (var action in _actions)
      {
        System.Console.WriteLine($"[TASK] = {action.Value.Description}");
      }

      System.Console.WriteLine("Example: cec.exe import#ImporterCec5#C:\\mycorpus.cec5 frequency3 POS Lemma Wort");
      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [SCRIPTING] --- :>");
      System.Console.WriteLine();
      System.Console.WriteLine("All tasks above can be stored in a file to build up a automatic process.");
      System.Console.WriteLine("In this case it's recommended to redirect the [TASK]-output to a file and not to stdout");
      System.Console.WriteLine("Example: import#ImporterCec5#C:\\mycorpus.cec5 frequency3 POS Lemma Wort > output.csv");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [F:FORMAT] --- :>");
      System.Console.WriteLine();
      System.Console.WriteLine("If you use [TASK] or the scripting-mode [FILE: / DEBUG:], you can change the output format.");
      System.Console.WriteLine("You need to set one of the following tags as first parameter:");
      System.Console.WriteLine("F:TSV - (standard output format) tab separated values");
      System.Console.WriteLine("F:CSV - ';' separated values");
      System.Console.WriteLine("F:JSON - JSON-array");
      System.Console.WriteLine("F:SQL - SQL-statement");
      System.Console.WriteLine("Example: cec.exe F:JSON import#ImporterCec5#C:\\mycorpus.cec5 frequency3 POS Lemma Wort");
    }
  }
}