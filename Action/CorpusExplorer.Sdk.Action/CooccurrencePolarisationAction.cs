using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrencePolarisationAction : IAction
  {
    public string Action => "cooccurrence-polarisation";

    public string Description => Resources.DescCooccurrencePolarisation;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 3)
        return;

      var layer = args[0];
      var tokenA = args[1];
      var tokenB = args[2];

      var vm = new CooccurrencePolarisationViewModel
      {
        Selection = selection,
        LayerDisplayname = layer,
        LayerValueA = tokenA,
        LayerValueB = tokenB
      };
      vm.Execute();

      var dt = new DataTable();
      dt.Columns.Add(Resources.Cooccurrence, typeof(string));
      dt.Columns.Add(Resources.Significance, typeof(double));

      dt.BeginLoadData();
      foreach (var x in vm.CollocatesPolarisation)
        dt.Rows.Add(x.Key, x.Value);
      
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}