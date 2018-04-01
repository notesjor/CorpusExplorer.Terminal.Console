using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaDocumentAction : AbstractAction
  {
    public override string Action => "meta-by-document";
    public override string Description => "meta-by-document - list all documents with meta-data";

    public override void Execute(Selection selection, string[] args)
    {
      var columns = selection.GetDocumentMetadataPrototypeOnlyPropertiesAndTypes().ToArray();

      var dt = new DataTable();
      dt.Columns.Add("GUID", typeof(string));
      foreach (var column in columns)
        dt.Columns.Add(column.Key, column.Value);

      dt.BeginLoadData();
      foreach (var pair in selection.DocumentMetadata)
      {
        var items = new List<object> { pair.Key.ToString() };
        items.AddRange(columns.Select(column => pair.Value.ContainsKey(column.Key) ? pair.Value[column.Key] : null));
        dt.Rows.Add(items.ToArray());
      }
      dt.EndLoadData();

      WriteTable(dt);
    }
  }
}