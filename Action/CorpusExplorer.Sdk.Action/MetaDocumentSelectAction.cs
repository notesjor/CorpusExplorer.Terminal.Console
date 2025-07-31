using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaDocumentSelectAction : IAction
  {
    public string Action => "meta-by-document-select";
    public string Description => CorpusExplorer.Sdk.Action.Properties.Resources.DescMetaDocumentSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if(args.Length == 0)
        return;

      var dt = new DataTable();
      dt.Columns.Add(Resources.Guid, typeof(string));
      foreach (var x in args)
        dt.Columns.Add(x, typeof(string));

      var dict = new Dictionary<string, int>();
      for (var i = 0; i < args.Length; i++)
        dict.Add(args[i], i + 1);

      dt.BeginLoadData();
      foreach (var pair in selection.DocumentMetadata)
      {
        var row = new string[args.Length + 1];
        row[0] = pair.Key.ToString("N");

        foreach (var x in pair.Value)
          if (dict.ContainsKey(x.Key))
            row[dict[x.Key]] = x.Value?.ToStringSafe();

        dt.Rows.Add(row);
      }

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}