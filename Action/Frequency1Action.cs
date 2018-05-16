using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency1Action : AbstractAction
  {
    public override string Action => "frequency1";
    public override string Description => "frequency1 [LAYER1] - count token frequency on 1 [LAYER]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency1LayerViewModel {Selection = selection};
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Analyse();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}