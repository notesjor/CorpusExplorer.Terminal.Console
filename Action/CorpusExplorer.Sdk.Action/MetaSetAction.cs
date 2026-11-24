using System;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaSetAction : IAction
  {
    public string Action => "meta-set";
    public string Description => Resources.DescMetaSet;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 3)
        return;

      var metaName = args[0];
      var metaValue = args[1];
      
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

      var meta = selection.DocumentMetadata.ToDictionary(x => x.Key, x => x.Value.ToDictionary(y => y.Key, y => y.Value));
      
      var keys = meta.Keys.ToArray();
      foreach (var key in keys)
      {
        if (meta[key].ContainsKey(metaName))
          meta[key].Remove(metaName);
        meta[key].Add(metaName, metaValue);
      }

      selection.ResetAllDocumentMetadata(meta);

      exporter.Export(selection, path);
    }
  }
}