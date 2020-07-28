using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentTermFrequencyAction : IAction
  {
    public string Action => "tf";
    public string Description => Resources.DescTf;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DocumentTermFrequencyViewModel { Selection = selection, MetadataKey = args[0] };
      if (args.Length == 2)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}