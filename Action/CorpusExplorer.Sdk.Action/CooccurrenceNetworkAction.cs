using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceNetworkAction: IAction
  {
    public string Action => "cooccurrence-network";

    public string Description => "cooccurrence-network [LAYER] {minSIGNI} {minFREQ} - significant cooccurrence-network for all [LAYER] values";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new CooccurrenceViewModel { Selection = selection };
      if (args.Length >= 1)
        vm.LayerDisplayname = args[0];
      if (args.Length >= 2)
        vm.CooccurrenceMinSignificance = double.Parse(args[1]);
      if (args.Length >= 3)
        vm.CooccurrenceMinFrequency = int.Parse(args[2]);
      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(vm.SignificanceDictionary));
    }
  }
}
