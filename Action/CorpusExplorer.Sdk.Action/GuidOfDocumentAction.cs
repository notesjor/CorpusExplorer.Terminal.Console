using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class GuidOfDocumentAction : IAction
  {
    public string Action => "guid-of-document";
    public string Description => "guid-of-document - get title/guid-pairs of all documents (only if title is present as metadata)";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("Name", typeof(string));
      dt.Columns.Add("Guid", typeof(string));

      dt.BeginLoadData();
      foreach(var x in selection.DocumentGuidsAndDisplaynames)
        dt.Rows.Add(x.Value, x.Key);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}