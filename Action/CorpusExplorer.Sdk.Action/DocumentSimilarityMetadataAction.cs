using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentSimilarityMetadataAction : IAction
  {
    public string Action => "similarity-meta";
    public string Description => "similarity-meta [META] {LAYER} - [META] similarity based on {LAYER} (default: WORT)";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DocumentSimilarityMetadataViewModel { Selection = selection, MetadataKey = args[0] };
      if (args.Length == 2)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}