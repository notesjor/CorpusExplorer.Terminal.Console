using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CutOffPhraseAction : IAction
  {
    public string Action => "cutoff-phrase";

    public string Description => "cutoff-phrase [LAYER1] [QUERY 1] [LAYER2] [QUERY 2] - search on [LAYER1/2] in one sentence and prints out the sequence betweet [Q1/2]";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if(args.Length != 4)
        return;

      var vm = new CutOffPhraseViewModel { Selection = selection };
      vm.LayerDisplayname1 = args[0];
      vm.LayerQuery1 = args[1];
      vm.LayerDisplayname2 = args[2];
      vm.LayerQuery2 = args[3];
      vm.Execute();

      writer.WriteTable(vm.GetUniqueDataTableCutOffPhrase());
    }
  }
}
