using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class Frequency3Action : IAction
  {
    public string Action => "frequency3";
    public string Description => Resources.DescFrequency3;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency3LayerViewModel {Selection = selection};
      if (args != null && args.Length == 3)
      {
        vm.Layer1Displayname = args[0];
        vm.Layer2Displayname = args[1];
        vm.Layer3Displayname = args[2];
      }

      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetNormalizedDataTable());
    }
  }
}