using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CorpusExplorer.Port.RProgramming.Api.Action;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Port.RProgramming.Api.Importer;
using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;

namespace CorpusExplorer.Port.RProgramming.Api
{
  internal class Program
  {
    private static readonly AbstractAction[] _actions =
    {
      new Frequency1Action(),
      new Frequency2Action(),
      new Frequency3Action(),
      new CooccurrenceAction(),
      new MetaAction(),
      new CrossFrequencyAction(),
      new ConvertAction(),
      new FilterAction()
    };

    private static readonly AbstractImporter[] _importer =
    {
      new ImporterCec5(),
      new ImporterCec6(),
      new ImporterDtaBf(),
      new ImporterWeblicht()
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
      if (args == null || args.Length == 0)
        return;

      CorpusExplorerEcosystem.InitializeMinimal();

      var corpus = LoadCorpus(args[0]);
      var selection = corpus?.ToSelection();
      if (selection == null || selection.CountToken == 0)
        return;

      Console.OutputEncoding = Encoding.UTF8;

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
      var importer = _importer.FirstOrDefault(x => path.ToLower().EndsWith(x.FileExtension));
      return importer?.Import(path);
    }

    private static void Main(string[] args)
    {
      AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
      Execute(args);
    }
  }
}