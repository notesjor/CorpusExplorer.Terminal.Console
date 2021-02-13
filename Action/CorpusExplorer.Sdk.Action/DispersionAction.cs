using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DispersionAction : IAction
  {
    public string Action => "dispersion";
    public string Description => "dispersion [LAYER] [META] - calculates dispersions values of all [LAYER] values based on [META]";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DispersionViewModel { Selection = selection };
      if (args.Length > 0)
        vm.LayerDisplayname = args[0];
      if (args.Length > 1)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}
