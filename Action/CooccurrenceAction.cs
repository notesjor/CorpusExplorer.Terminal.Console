using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class CooccurrenceAction : AbstractAction
  {
    public override string Action => "cooccurrence";
    public override string Description => "cooccurrence [LAYER1] [minSIGNI] [minFREQ] - significant cooccurrences for all [LAYER] values";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new CooccurrenceViewModel {Selection = selection};
      if (args.Length >= 1)
        vm.LayerDisplayname = args[0];
      if (args.Length >= 2)
        vm.CooccurrenceMinSignificance = double.Parse(args[1]);        
      if (args.Length >= 3)
        vm.CooccurrenceMinFrequency = int.Parse(args[2]);
      vm.Execute();
      
      writer.WriteTable(selection.Displayname, vm.GetFullDataTable());
    }
  }
}