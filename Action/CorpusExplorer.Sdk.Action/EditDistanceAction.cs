using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class EditDistanceAction : IAction
  {
    public string Action => "editdist";

    public string Description => Resources.editdist_desc;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new EditDistantCalculationViewModel { Selection = selection };
      if (args.Length >= 1)
        vm.LayerDisplayname = args[0];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}
