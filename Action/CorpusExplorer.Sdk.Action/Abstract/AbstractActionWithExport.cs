using System;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Exporter.Abstract;

namespace CorpusExplorer.Sdk.Action.Abstract
{
  public abstract class AbstractActionWithExport : IAction
  {
    public abstract string Action { get; }
    public abstract string Description { get; }
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (GetExporter(args, out var exporter, out var path))
        return;

      args = args.Take(args.Length - 1).ToArray();

      var corpus = ExecuteCall(selection, args, path);
      if(corpus == null)
        return;

      exporter.Export(corpus, path);
    }

    public void ExecuteXmlScriptProcessorBypass(Selection selection, string[] args, AbstractExporter exporter, string path)
    {
      var corpus = ExecuteCall(selection, args, path);
      if (corpus == null)
        return;

      exporter.Export(corpus, path);
    }

    private static bool GetExporter(string[] args, out AbstractExporter exporter, out string path)
    {
      exporter = null;
      path = null;

      var output = args.Last().Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries);
      if (output.Length != 2)
        return true;

      exporter = Configuration.AddonExporters.GetReflectedType(output[0], "Exporter");
      if (exporter == null)
        return true;

      path = output[1].Replace("\"", "");
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);
      
      return false;
    }

    /// <summary>
    /// Führt die Aktion aus - das zurückgegebene Korpus wird dann exportiert
    /// </summary>
    /// <param name="selection">Ausgangsselection</param>
    /// <param name="args">Argumente (Export wurde bereits entfernt)</param>
    /// <param name="path">Pfad der später zu exportierenden Datei. Wird übergeben, falls Path.GetFileName genutzt werden soll.</param>
    /// <returns>Korpus</returns>
    protected abstract AbstractCorpusAdapter ExecuteCall(Selection selection, string[] args, string path);
  }
}
