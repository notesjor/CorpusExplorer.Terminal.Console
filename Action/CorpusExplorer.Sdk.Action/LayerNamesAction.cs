using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class LayerNamesAction : IAction
  {
    public string Action => "layer-names";
    public string Description => Resources.DescLayerNames;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Layernames, typeof(string));

      dt.BeginLoadData();
      foreach (var x in selection.LayerDisplaynames)
        dt.Rows.Add(x);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}