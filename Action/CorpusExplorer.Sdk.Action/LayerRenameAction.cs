using System;
using System.IO;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class LayerRenameAction : IAction
  {
    public string Action => "layer-rename";
    public string Description => CorpusExplorer.Sdk.Action.Properties.Resources.DescLayerRename;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 3)
        return;
        
      var output = args[2].Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries);
      if (output.Length != 2)
        return;

      var exporter = Configuration.AddonExporters.GetReflectedType(output[0], "Exporter");
      if (exporter == null)
        return;

      var path = output[1].Replace("\"", "");
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      var corpus = selection.ToCorpus();
      corpus.LayerRename(args[0], args[1]);

      exporter.Export(corpus, path);
    }
  }
}