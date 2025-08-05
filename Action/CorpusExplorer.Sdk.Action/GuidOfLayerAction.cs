using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class GuidOfLayerAction : IAction
  {
    public string Action => "guid-of-layer";
    public string Description => "guid-of-layer - get name/guid-pairs of all layers";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("Name", typeof(string));
      dt.Columns.Add("Guid", typeof(string));

      dt.BeginLoadData();
      foreach(var x in selection.LayerGuidAndDisplaynames)
        dt.Rows.Add(x.Value, x.Key);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}