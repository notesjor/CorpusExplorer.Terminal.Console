using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class InverseDocumentFrequencyAction : IAction
  {
    public string Action => "idf";
    public string Description => Resources.DescIdf;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new InverseDocumentFrequencyViewModel { Selection = selection, MetadataKey = args[0]};
      if (args.Length == 2)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}