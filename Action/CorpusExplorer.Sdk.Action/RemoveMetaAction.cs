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
  public class RemoveMetaAction : IAction
  {
    public string Action => "remove-meta";
    public string Description => Resources.DescMetaRemove;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 2)
        return;

      var meta = args[0];
      selection.RemoveDocumentMetadata(meta);

      var output = args[1].Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries);
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