using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class Frequency2Action : IAction
  {
    public string Action => "frequency2";
    public string Description => Resources.DescFrequency2;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency2LayerViewModel {Selection = selection};
      if (args != null && args.Length == 2)
      {
        vm.Layer1Displayname = args[0];
        vm.Layer2Displayname = args[1];
      }

      vm.Execute();
      writer.WriteTable(selection.Displayname, vm.GetNormalizedDataTable());
    }
  }
}