using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaSelectedAction : IAction
  {
    public string Action => "meta-select";
    public string Description => Resources.MetaSelectedActionDescription;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length == 0)
        return;

      var vm = new CorpusWeightUnlimmitedViewModel { Selection = selection };
      vm.Execute();
      writer.WriteTable(selection.Displayname, vm.GetFilteredDataTable(args));
    }
  }
}