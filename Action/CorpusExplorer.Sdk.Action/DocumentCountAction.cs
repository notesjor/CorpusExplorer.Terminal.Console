using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentCountAction : IAction
  {
    public string Action => "how-many-documents";
    public string Description => Resources.DescDocumentCount;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Param, typeof(string));
      dt.Columns.Add(Resources.Value, typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add(Resources.Documents, (double) selection.CountDocuments);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}