﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model.Cache;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Web;
using CorpusExplorer.Terminal.Console.Xml.Processor;

namespace CorpusExplorer.Terminal.Console
{
  public class Program
  {
    // private static readonly Dictionary<string, IAction> _actions = new Dictionary<string, IAction>();

    private static readonly string _appPath =
      Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cec.exe");

    private static readonly Dictionary<string, AbstractTableWriter> _formats =
      new Dictionary<string, AbstractTableWriter>
      {
        {"F:TSV", new TsvTableWriter()},
        {"F:CSV", new CsvTableWriter()},
        {"F:XML", new XmlTableWriter()},
        {"F:SQL", new SqlTableWriter()},
        {"F:SQLSCHEMA", new SqlSchemaOnlyTableWriter()},
        {"F:SQLDATA", new SqlDataOnlyTableWriter()},
        {"F:JSON", new JsonTableWriter()},
        {"F:HTML", new HtmlTableWriter()}
      };

    private static AbstractTableWriter _writer = new TsvTableWriter();

    private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      try
      {
        var dll = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.Ordinal)) + ".dll";
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

    private static void DebugSkript(string[] args)
    {
      var path = args[0].Replace("DEBUG:", "").Replace("\"", "");
      if (ProcessXmlScript(path))
        return;

      var lines = File.ReadAllLines(path, Configuration.Encoding);

      System.Console.WriteLine($"execute script: {path}");
      for (var i = 0; i < lines.Length; i++)
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

    private static void Execute(string[] args)
    {
      if (args == null || args.Length == 0)
      {
        PrintHelp();
        return;
      }

      if (args[0].StartsWith("F:"))
      {
        _writer = _formats.ContainsKey(args[0]) ? _formats[args[0]] : _formats.First().Value;

        var list = new List<string>(args);
        list.RemoveAt(0);
        args = list.ToArray();
      }

      if (args[0].StartsWith("PORT:") || args[0].StartsWith("IP:"))
      {
        ExecuteWebservice(args);
        return;
      }

      if (args[0].StartsWith("FILE:"))
      {
        ExecuteSkript(args);
        return;
      }

      if (args[0].StartsWith("DEBUG:"))
      {
        DebugSkript(args);
        return;
      }

      if (args[0].ToLowerInvariant() == "shell")
      {
        ExecuteShell();
        _writer.Destroy();
        return;
      }

      if (args.Length == 1 && File.Exists(args[0]))
      {
        args[0] = "FILE:" + args[0];
        ExecuteSkript(args);
        return;
      }

      ExecuteDirect(args);
      _writer.Destroy();
    }

    private static void ExecuteWebservice(string[] args)
    {
      ConsoleHelper.PrintHeader();

      var ip = "127.0.0.1";
      var port = 2312;
      var file = "";
      var timeout = 30000;

      foreach (var arg in args)
      {
        if (arg.StartsWith("IP:"))
          ip = arg.Replace("IP:", "");
        else if (arg.StartsWith("PORT:"))
          port = int.Parse(arg.Replace("PORT:", ""));
        else if (arg.StartsWith("TIMEOUT:"))
          timeout = int.Parse(arg.Replace("TIMEOUT:", ""));
        else
          file = arg;
      }

      if (string.IsNullOrEmpty(file) || !file.Contains("#"))
      {
        var ws = new WebServiceDirect(_writer, ip, port, timeout);
        ws.Run();
      }
      else
      {
        var ws = new WebService(_writer, ip, port, file, timeout);
        ws.Run();
      }
    }

    private static void ExecuteDirect(string[] args)
    {
      var task = args[1].ToLowerInvariant();
      var action = Configuration.GetConsoleAction(task);
      if (action == null)
        return;

      var corpus = CorpusLoadHelper.LoadCorpus(args[0]);
      var selection = corpus?.ToSelection();
      if (selection == null || selection.CountToken == 0)
        return;

      System.Console.OutputEncoding = Configuration.Encoding;
      var temp = args.ToList();
      temp.RemoveAt(0); // CorpusFile (no longer needed)
      temp.RemoveAt(0); // Action (no longer needed)
      action.Execute(selection, temp.ToArray(), _writer);
    }

    private static void ExecuteShell()
    {
      ConsoleHelper.PrintHeader();
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
        if (command == null)
          continue;

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
            {
              File.WriteAllLines(command.Replace("SAVE:", ""), history.ToArray());
            }
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

    private static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      CorpusExplorerEcosystem.Initialize(new CacheStrategyDisableCaching());
      Execute(args);
    }

    private static void PrintHelp()
    {
      ConsoleHelper.PrintHeader();
      System.Console.WriteLine("Syntax for annotation/conversion:");
      System.Console.WriteLine("cec.exe [INPUT] convert [OUTPUT]");
      System.Console.WriteLine("Syntax for filtering:");
      System.Console.WriteLine("cec.exe [INPUT] [QUERY] [OUTPUT]");
      System.Console.WriteLine("Syntax for analytics (writes output to stdout):");
      System.Console.WriteLine("cec.exe {F:FORMAT} [INPUT] [ACTION]");
      System.Console.WriteLine("Syntax for scripting:");
      System.Console.WriteLine("cec.exe FILE:[PATH]");
      System.Console.WriteLine("More detailed scripting errors:");
      System.Console.WriteLine("cec.exe DEBUG:[PATH]");
      System.Console.WriteLine("To start interactive shell mode");
      System.Console.WriteLine("cec.exe SHELL");
      System.Console.WriteLine("To start a web-service");
      System.Console.WriteLine("cec.exe {F:FORMAT} PORT:2312 {IP:127.0.0.1} {TIMEOUT:30000} {INPUT}");
      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [INPUT] --- :>");
      System.Console.WriteLine();
      System.Console.WriteLine("Import corpus material - direct [INPUT]:");

      var importer = Configuration.AddonImporters.GetReflectedTypeNameDictionary();
      foreach (var x in importer) System.Console.WriteLine($"[INPUT] = import#{x.Key}#[FILES]");

      System.Console.WriteLine("Note: [FILES] = separate files with & - merges all files before processing");
      System.Console.WriteLine(
                               "Example: cec.exe import#ImporterCec5#C:\\mycorpus1.cec5&C:\\mycorpus2.cec5 convert ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();

      var scraper = Configuration.AddonScrapers.GetReflectedTypeNameDictionary();
      System.Console.WriteLine();
      System.Console.WriteLine("Annotate raw text - indirect [INPUT]:");
      foreach (var x in scraper)
        System.Console.WriteLine($"[INPUT] = annotate#{x.Key}#[TAGGER]#[LANGUAGE]#[DIRECTORY]");
      System.Console.WriteLine("Note: [DIRECTORY] = any directory you like - all files will be processed");
      var tagger = Configuration.AddonTaggers.GetReflectedTypeNameDictionary();
      System.Console.WriteLine("[TAGGER] & [LANGUAGE]:");
      foreach (var x in tagger)
      {
        System.Console.Write($"[TAGGER] = {x.Key} ");
        System.Console.WriteLine($"([LANGUAGE] = {string.Join(", ", x.Value.LanguagesAvailabel)})");
      }

      System.Console.WriteLine(
                               "Example: cec.exe annotate#DpxcScraper#SimpleTreeTagger#Deutsch#C:\\dpxc\\ convert ExporterCec6#C:\\mycorpus.cec6");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [OUTPUT] --- :>");
      System.Console.WriteLine();
      var exporter = Configuration.AddonExporters.GetReflectedTypeNameDictionary();
      System.Console.WriteLine("[OUTPUT-EXPORTER] - for query or convert:");
      foreach (var x in exporter)
        System.Console.WriteLine($"[OUTPUT] = {x.Key}#[FILE]");

      System.Console.WriteLine("Note: [FILE] = any file you like to store the output");
      System.Console
            .WriteLine("Example 'convert': cec.exe import#ImporterCec5#C:\\mycorpus.cec5 convert ExporterCec6#C:\\mycorpus.cec6");
      System.Console
            .WriteLine("Example 'query': cec.exe import#ImporterCec5#C:\\mycorpus.cec5 query !M:Author::Jan ExporterCec6#C:\\mycorpus.cec6");

      System.Console.WriteLine();
      System.Console.WriteLine("[QUERY]:");
      System.Console.WriteLine("A preceding ! inverts the entiere query");
      System.Console.WriteLine("First character:");
      System.Console.WriteLine("M = Metadata -OR- T = (Full)Text -OR- X = Extended Features");
      System.Console.WriteLine("followed by configuration (see below), the :: separator and the values");
      System.Console.WriteLine();
      System.Console.WriteLine("Second character [OPERATOR] (if you choose M):");
      System.Console.WriteLine("  ? = regEx | : = contains (case sensitive) | . = contains (not case sensitive)");
      System.Console
            .WriteLine("  = = match exact (case sensitive) | - = match exact (not case sensitive) | ! = is empty");
      System.Console.WriteLine("  ( = starts with (case sensitive) | ) = ends with (case sensitive)");
      System.Console
            .WriteLine("If you have chosen M - enter the name of the meta category (see [ACTION] = meta-categories)");
      System.Console
            .WriteLine("Example (query only): !M:Author::Jan - Finds all documents where \"Jan\" isn't an author");
      System.Console
            .WriteLine("Example (in action): cec.exe import#ImporterCec6#C:\\mycorpus.cec6 query !M:Author::Jan ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();
      System.Console.WriteLine("Second character [OPERATOR] (if you choose T):");
      System.Console
            .WriteLine("  ~ = any match | - = all in one document | = = all in one sentence | § = exact phrase");
      System.Console
            .WriteLine("  ? = regEx value | F = regEx fulltext-search (very slow) | 1 = first plus any other match");
      System.Console.WriteLine("If you have chosen T - enter the layer name (see [ACTION] = layer-names)");
      System.Console
            .WriteLine("Example (query only): T§Wort::OpenSource;Software - Finds all documents with the exact phrase \"OpensSource Software\"");
      System.Console
            .WriteLine("Example (in action): cec.exe import#ImporterCec6#C:\\mycorpus.cec6 query T§Wort::OpenSource;Software ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("Note 1: If you use several words in a T-query, then separate them with ;");
      System.Console.WriteLine("Note 2: You can also use a query file (*.ceusd) - use the FILE: prefix");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 query FILE:C:\\query.ceusd ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine();
      System.Console.WriteLine("Second character [OPERATOR] (if you choose X):");
      System.Console.WriteLine("  R = random selection | S = auto split by meta-data (use cluster for auto split)");
      System.Console.WriteLine("If you use XR for random selection you need to specify the document count");
      System.Console.WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 query XR::100 frequency1 Wort");
      System.Console.WriteLine("Note 4: XR will generate two outputs - the regular and the inverted output.");
      System.Console
            .WriteLine("If you have chosen XS - enter the name of the meta category (see [ACTION] = meta-categories)");
      System.Console.WriteLine();
      System.Console.WriteLine("Enter the separator :: followed by the query");
      System.Console.WriteLine();

      System.Console.WriteLine("If you use XS you must specify the meta data type - TEXT, INT, FLOAT or DATE");
      System.Console.WriteLine("Note 5: XS will generate multiple outputs - based on clusters.");
      System.Console.WriteLine("TEXT generates for every entry a separate snapshot");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 cluster XSAuthor::TEXT frequency1 Wort");
      System.Console.WriteLine("INT / FLOAT you need to set up a [CLUSTERSIZE]");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 cluster XSYear::INT;10 ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("DATE;C;[CLUSTERSIZE] - generates [CLUSTERSIZE] clusters.");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 cluster XSDate::DATE;C;10 ExporterCec6#C:\\mycorpus.cec6");
      System.Console.WriteLine("DATE;CEN = Century-Cluster / DATE;DEC = Decate-Cluster / DATE;Y = Year-Cluster");
      System.Console
            .WriteLine("DATE;YW = Week-Cluster / DATE;YM = Year/Month-Cluster / DATE;YMD = Year/Month/Day-Cluster");
      System.Console
            .WriteLine("DATE;YMDH = Year/Month/Day/Hour-Cluster / DATE;YMDHM = Year/Month/Day/Hour/Minute-Cluster / ALL = Every-Time-Cluster");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 cluster XSDate::DATE;YMD ExporterCec6#C:\\mycorpus.cec6");
      System.Console
            .WriteLine("WINDOW = Add WINDOW + SIZE as an prefix for each cluster argument to enable the rolling window feature");
      System.Console
            .WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 cluster XSDate::WINDOW7;DATE;YMD ExporterCec6#C:\\mycorpus.cec6");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [ACTION] --- :>");
      System.Console.WriteLine();
      System.Console
            .WriteLine("Most actions accept arguments. [ARG] is a requiered argument. {ARG} is an optional argument.");

      foreach (var action in Configuration.AddonConsoleActions.OrderBy(x => x.Action))
        System.Console.WriteLine($"[ACTION] = {action.Description}");

      System.Console.WriteLine("Example: cec.exe import#ImporterCec6#C:\\mycorpus.cec6 frequency3 POS Lemma Wort");
      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [SCRIPTING] --- :>");
      System.Console.WriteLine();
      System.Console.WriteLine("All actionss above can be stored in a file to build up a automatic process.");
      System.Console
            .WriteLine("In this case it's recommended to redirect the [ACTION]-output to a file and not to stdout");
      System.Console.WriteLine("Example: import#ImporterCec6#C:\\mycorpus.cec6 frequency3 POS Lemma Wort > output.csv");

      System.Console.WriteLine();
      System.Console.WriteLine();
      System.Console.WriteLine("<: --- [F:FORMAT] --- :>");
      System.Console.WriteLine();
      System.Console
            .WriteLine("If you use [ACTION] or the scripting-mode [FILE: / DEBUG:], you can change the output format.");
      System.Console.WriteLine("You need to set one of the following tags as first parameter:");
      System.Console.WriteLine("F:TSV - (standard output format) tab separated values");
      System.Console.WriteLine("F:CSV - ';' separated values");
      System.Console.WriteLine("F:JSON - JSON-array");
      System.Console.WriteLine("F:XML - XML-Document");
      System.Console.WriteLine("F:HTML - HTML5-Document");
      System.Console.WriteLine("F:SQL - SQL-statement");
      System.Console
            .WriteLine("Example: cec.exe F:JSON import#ImporterCec6#C:\\mycorpus.cec6 frequency3 POS Lemma Wort");
    }

    private static bool ProcessXmlScript(string path)
    {
      try
      {
        XmlScriptProcessor.Process(path, _formats);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static void StartProcessCec(string argument)
    {
      if (string.IsNullOrEmpty(argument))
        return;
      if (argument.StartsWith("#"))
      {
        System.Console.WriteLine(argument);
        return;
      }

      if (argument.Contains(" > "))
      {
        var split = argument.Split(new[] { " > " }, StringSplitOptions.None).ToList();
        argument = split[0];

        var process = Process.Start(new ProcessStartInfo
        {
          Arguments = !argument.StartsWith("F:") ? $"{_writer.TableWriterTag} {argument}" : argument,
          CreateNoWindow = true,
          FileName = _appPath,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardOutput = true,
          StandardOutputEncoding = Configuration.Encoding,
          UseShellExecute = false
        });

        var res = process?.StandardOutput.ReadToEnd();
        process?.WaitForExit();

        File.WriteAllText(split[1], res, Configuration.Encoding);
      }
      else
      {
        var process = Process.Start(new ProcessStartInfo
        {
          Arguments = !argument.StartsWith("F:") ? $"{_writer.TableWriterTag} {argument}" : argument,
          CreateNoWindow = true,
          FileName = _appPath,
          WindowStyle = ProcessWindowStyle.Hidden,
          RedirectStandardOutput = true,
          StandardOutputEncoding = Configuration.Encoding,
          UseShellExecute = false
        });
        var res = process?.StandardOutput.ReadToEnd();
        process?.WaitForExit();

        System.Console.Out.Write(res);
      }
    }
  }
}