using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System.Linq;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceNetworkSelectAction : IAction
  {
    public string Action => "cooccurrence-network-select";

    public string Description => "cooccurrence-network-select [LAYER] [minSIGNI] [minFREQ] [WORDS] - significant cooccurrence-network for all [LAYER] values";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new CooccurrenceNetworkSelectiveViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        CooccurrenceMinSignificance = double.Parse(args[1]),
        CooccurrenceMinFrequency = int.Parse(args[2]),
        LayerQueries = args.Skip(3)
      };
      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(vm.SignificanceDictionary));
    }
  }
}
