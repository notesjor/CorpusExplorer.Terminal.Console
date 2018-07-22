using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class CrossFrequencyAction : AbstractAction
  {
    public override string Action => "cross-frequency";
    public override string Description => "cross-frequency [LAYER] - calculates the cross-frequency based on [LAYER]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new FrequencyCrossViewModel {Selection = selection};
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}