using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency3Action : AbstractAction
  {
    public override string Action => "frequency3";
    public override string Description => "frequency3 [LAYER1] [LAYER2] [LAYER3] - count token frequency on 3 layers";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency3LayerViewModel {Selection = selection};
      if (args != null && args.Length == 3)
      {
        vm.Layer1Displayname = args[0];
        vm.Layer2Displayname = args[1];
        vm.Layer3Displayname = args[2];
      }

      vm.Analyse();

      writer.WriteTable(vm.GetNormalizedDataTable());
    }
  }
}