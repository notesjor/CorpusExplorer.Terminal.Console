using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class TermFrequencyInverseDocumentFrequencyAction : IAction
  {
    public string Action => "tf-idf";
    public string Description => Resources.DescTfIdf;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new TermDocumentMatrixViewModel { Selection = selection, MetadataKey = args[0] };
      if (args.Length == 2)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}