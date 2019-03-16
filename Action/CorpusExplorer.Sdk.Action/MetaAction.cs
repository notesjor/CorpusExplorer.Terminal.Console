using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaAction : IAction
  {
    public string Action => "meta";
    public string Description => Resources.DescMeta;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new CorpusWeightUnlimmitedViewModel {Selection = selection};
      vm.Execute();
      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}