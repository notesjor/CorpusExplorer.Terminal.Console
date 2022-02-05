using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaDocumentAction : IAction
  {
    public string Action => "meta-by-document";
    public string Description => Resources.DescMetaByDocument;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var columns = selection.GetDocumentMetadataPrototypeOnlyPropertiesAndTypes().ToArray();

      var dt = new DataTable();
      dt.Columns.Add(Resources.Guid, typeof(string));
      foreach (var column in columns)
        dt.Columns.Add(column.Key, column.Value);

      dt.BeginLoadData();
      foreach (var pair in selection.DocumentMetadata)
      {
        var items = new List<object> {pair.Key.ToString("N")};
        items.AddRange(columns.Select(column => pair.Value.ContainsKey(column.Key) ? pair.Value[column.Key] : null));
        dt.Rows.Add(items.ToArray());
      }

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}