using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System.Data;

namespace CorpusExplorer.Sdk.Action
{
  public class GuidOfCorpusAction : IAction
  {
    public string Action => "guid-of-corpus";
    public string Description => "guid-of-corpus - get name/guid-pairs of all corpora in selection";

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