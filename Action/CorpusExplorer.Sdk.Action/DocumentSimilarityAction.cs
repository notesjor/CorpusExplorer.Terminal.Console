using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentSimilarityAction : IAction
  {
    public string Action => "similarity";
    public string Description => "similarity {LAYER} - document similarity based on {LAYER} (default: WORT)";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DocumentSimilarityMetadataViewModel { Selection = selection };
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}