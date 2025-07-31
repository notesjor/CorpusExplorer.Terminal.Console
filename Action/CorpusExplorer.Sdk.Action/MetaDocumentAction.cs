using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaDocumentAction : IAction
  {
    public string Action => "meta-by-document";
    public string Description => Resources.DescMetaByDocument;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var columns = new HashSet<string>(selection.GetDocumentMetadataPrototypeOnlyProperties());
      if (columns.Contains(Resources.Guid))
        columns.Remove(Resources.Guid);

      var dt = new DataTable();
      dt.Columns.Add(Resources.Guid, typeof(string));
      foreach (var column in columns)
        dt.Columns.Add(column, typeof(string));

      dt.BeginLoadData();
      foreach (var pair in selection.DocumentMetadata)
      {
        var items = new List<object> { pair.Key.ToString("N") };
        items.AddRange(columns.Select(x => pair.Value.TryGetValue(x, out var v) ? v?.ToStringSafe() : null));
        dt.Rows.Add(items.ToArray());
      }

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}