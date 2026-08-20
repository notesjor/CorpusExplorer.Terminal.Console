using System;
using System.IO;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaRenameAction : IAction
  {
    public string Action => "meta-rename";
    public string Description => Resources.DescMetaRename;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 3)
        return;

      var metaOld = args[0];
      var metaNew = args[1];
      selection.RenameDocumentMetadata(metaOld, metaNew);

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

      exporter.Export(selection, path);
    }
  }
}