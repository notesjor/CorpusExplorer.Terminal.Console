using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class GuidOfSelectionAction : IAction
  {
    public string Action => "guid-of-selection";
    public string Description => "guid-of-selection - get name/guid-pair of the current selection (note: temporary in cluster)";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("Name", typeof(string));
      dt.Columns.Add("Guid", typeof(string));

      dt.BeginLoadData();
      foreach(var x in selection.CorporaGuidsAndDisplaynames)
        dt.Rows.Add(x.Value, x.Key);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}