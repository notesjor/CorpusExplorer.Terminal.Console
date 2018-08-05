using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class NGramAction : AbstractAction
  {
    public override string Action => "n-gram";
    public override string Description => "n-gram [N] [LAYER] [minFREQ] - [N] sized N-gram based on [LAYER]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new NgramViewModel {Selection = selection};
      if (args.Length == 0)
        vm.NGramSize = 5;
      if (args.Length >= 1)
        vm.NGramSize = int.Parse(args[0]);
      if (args.Length >= 2)
        vm.LayerDisplayname = args[1];
      if (args.Length == 3)
        vm.NGramMinFrequency = int.Parse(args[2]);
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}