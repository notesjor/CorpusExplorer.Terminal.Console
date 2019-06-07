using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DisambiguationeAction : IAction
  {
    public string Action => "disambiguation";

    public string Description => Resources.DisambiguationeActionDescription;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 2)
        return;

      var vm = new DisambiguationViewModel { Selection = selection };
      vm.LayerDisplayname = args[0];
      vm.LayerQuery = args[1];
      vm.MinimumSignificance = 1;
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetFullDataTable());
    }
  }
}