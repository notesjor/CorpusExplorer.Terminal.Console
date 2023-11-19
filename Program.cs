using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model.Cache;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Properties;
#if Featherweight
#else 
using CorpusExplorer.Terminal.Console.Web;
#endif
using CorpusExplorer.Terminal.Console.Xml.Model;
using CorpusExplorer.Terminal.Console.Xml.Processor;

namespace CorpusExplorer.Terminal.Console
{
  public class Program
  {
    // private static readonly Dictionary<string, IAction> _actions = new Dictionary<string, IAction>();

    private static readonly string _appPath =
      Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cec.exe");

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

    private static void DebugScript(string[] args)
    {
      var path = args[0].Replace("DEBUG:", "").Replace("\"", "");
      if (ProcessXmlScript(path))
        return;

      var lines = File.ReadAllLines(path, Configuration.Encoding);

      System.Console.WriteLine(Resources.ExecuteScript, path);
      for (var i = 0; i < lines.Length; i++)
        try
        {
          var line = lines[i];
          System.Console.Write($"[{i + 1:D3}/{lines.Length:D3}] {line}");
          StartProcessCec(line);
          System.Console.WriteLine(Resources.PointPointPointOk);
        }
        catch (Exception ex)
        {
          System.Console.WriteLine(ex.Message);
          System.Console.WriteLine(ex.StackTrace);
        }
    }

    private static void Execute(string[] args)
    {
      if (args.Length > 0 && args[0] == "/WAIT")
      {
        System.Console.WriteLine("...PRESS ENTER TO CONTINUE...");
        System.Console.ReadLine();

        var tmp = args.ToList();
        tmp.RemoveAt(0);

        args = tmp.ToArray();
      }

      if (args == null || args.Length == 0)
      {
        PrintHelp(true);
        return;
      }

      if (args.Length == 1 && args[0] == "--github")
      {
        PrintDocs();
        return;
      }

      if (args[0].StartsWith("WAIT:"))
      {
        Wait(args[0]);
        var tmp = args.ToList();
        tmp.RemoveAt(0);
        args = tmp.ToArray();
      }

      if (args[0].StartsWith("F:") || args[0].StartsWith("FNT:"))
      {
        _writer = Configuration.GetTableWriter(args[0]);

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
        System.Console.WriteLine("CEC DEBUG-WAIT [PRESS -ENTER- TO CONTINUE]");
        System.Console.ReadLine();
        DebugScript(args);
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

      if (args.Length == 1 && args[0] == "--session")
      {
        SessionMode();
        return;
      }

      ExecuteDirect(args);
      _writer.Destroy();
    }

    private static void SessionMode()
    {
      var stream = System.Console.OpenStandardInput();
      var reader = new StreamReader(stream, Encoding.UTF8);
      var xml = reader.ReadToEnd();

      reader.Close();
      stream.Close();

      session session;
      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
      {
        var serializer = new XmlSerializer(typeof(session));
        session = (session)serializer.Deserialize(ms);
      }

      try
      {
        XmlScriptProcessor.ExecuteSession(session);
      }
      catch (Exception ex)
      {
        XmlScriptProcessor.LogError(ex);
      }
    }

    private static void Wait(string cmd)
    {
      var process = Process.GetProcessById(int.Parse(cmd.Replace("WAIT:", "")));
      System.Console.WriteLine($"WAIT 4: {process.Id} - {process.ProcessName}");
      process.WaitForExit();
    }

    private static void PrintDocs()
    {
      Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
      PrintHelp(false);
      System.Console.WriteLine("---");
      Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
      PrintHelp(false);
    }

    private static void ExecuteWebservice(string[] args)
    {
      ConsoleHelper.PrintHeader();

      var ip = "127.0.0.1";
      var port = 2312;
      var file = "";
      var timeout = 120;

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

      #if Featherweight
      #else
      if (file.Contains("#"))
      {
        var ws = new WebService(_writer, ip, port, file, timeout);
        ws.Run();
      }
      else
      {
        var ws = new WebService(_writer, ip, port, file, timeout);
        ws.Run();
      }
      #endif
    }

    private static void ExecuteDirect(string[] args)
    {
      var task = args[1].ToLowerInvariant();
      var action = Configuration.GetConsoleAction(task);
      if (action == null)
        return;

      var corpus = args[0] == "NULL" ? CorpusNullHelper.Corpus : CorpusLoadHelper.LoadCorpus(args[0]);
      var selection = corpus?.ToSelection();
      if (selection == null)
        return;

      var temp = args.ToList();
      temp.RemoveAt(0); // CorpusFile (no longer needed)
      temp.RemoveAt(0); // Action (no longer needed)
      action.Execute(selection, temp.ToArray(), _writer);
    }

    private static void ExecuteShell()
    {
      ConsoleHelper.PrintHeader();
      System.Console.WriteLine(Resources.BaseHelp);
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
            PrintHelp(true);
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
      System.Console.InputEncoding = System.Console.OutputEncoding = Configuration.Encoding;
      CultureInfo.DefaultThreadCurrentCulture = Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
      Execute(args);
    }

    private static void PrintHelp(bool clear)
    {
      ConsoleHelper.PrintHeader(clear);
      System.Console.WriteLine(Resources.HelpModes);
      System.Console.WriteLine(Resources.HelpImportHeader);

      var importer = Configuration.AddonImporters.GetReflectedTypeNameList("Importer");
      foreach (var x in importer) System.Console.WriteLine(Resources.HelpImportPattern, x);

      System.Console.WriteLine(Resources.HelpImportExample);

      var scraper = Configuration.AddonScrapers.GetReflectedTypeNameList("Scraper");
      System.Console.WriteLine(Resources.HelpAnnotateHeader);
      foreach (var x in scraper)
        System.Console.WriteLine(Resources.HelpAnnotatePattern, x);
      System.Console.WriteLine(Resources.HelpAnnotateNote);
      var tagger = Configuration.AddonTaggers.GetReflectedTypeNameList(""); // ignore hier nicht, da Tagger wie in TreeTagger Namensbestandteil ist. Ignore nur bei Namenssuche.
      System.Console.WriteLine(Resources.HelpAnnotateTaggerHeader);
      foreach (var x in tagger)
      {
        System.Console.Write(Resources.HelpAnnotateTaggerPattern, x);
        System.Console.WriteLine(Resources.HelpAnnotateTaggerLanguagePattern, string.Join(", ", Configuration.AddonTaggers.GetReflectedType(x, "Tagger").LanguagesAvailabel));
      }
      System.Console.WriteLine(Resources.HelpAnnotateExample);

      System.Console.WriteLine(Resources.HelpOutputHeader);
      var exporter = Configuration.AddonExporters.GetReflectedTypeNameList("Exporter");
      foreach (var x in exporter)
        System.Console.WriteLine(Resources.HelpOutputPattern, x);
      System.Console.WriteLine(Resources.HelpOutputExample);

      System.Console.WriteLine(Resources.HelpQueryClusterSyntax);

      System.Console.WriteLine(Resources.HelpActionHeader);
      foreach (var action in Configuration.AddonConsoleActions.OrderBy(x => x.Action))
        System.Console.WriteLine(Resources.HelpActionPattern, action.Description);
      System.Console.WriteLine(Resources.HelpActionExample);

      System.Console.WriteLine(Resources.HelpScripting);
      System.Console.WriteLine(Resources.HelpFormatHeader);
      foreach (var x in Configuration.AddonTableWriter.OrderBy(x => x.Value.WriteTid))
        System.Console.WriteLine($"{x.Value.TableWriterTag} - {x.Value.Description}");
      System.Console.WriteLine(Resources.HelpFormatFooter);
    }

    private static bool ProcessXmlScript(string path)
    {
      try
      {
        Task.Run(MyGcCollect);
        XmlScriptProcessor.Process(path);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private static async Task MyGcCollect()
    {
      while (true)
      {
        await Task.Delay(TimeSpan.FromMinutes(5));
        Task.Run(GC.Collect);
      }
    }

    private static string[] _separator = { " > " };

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
        var split = argument.Split(_separator, StringSplitOptions.None).ToList();
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