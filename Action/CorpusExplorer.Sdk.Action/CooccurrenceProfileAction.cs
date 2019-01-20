using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceProfileAction : IAction
  {
    public string Action => "cooccurrence-profile";

    public string Description =>
      "cooccurrence-profile [LAYER] [WORD] - significant cooccurrence profile for [WORD] on [LAYER].";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var vm = new CooccurrenceSecondLevelProfileViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        LayerValue = args[1]
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}