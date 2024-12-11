using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class TermDocumentMatrixAction : IAction
  {
    public string Action => "td-matrix";
    public string Description => "td-matrix [LAYER] [META] - calculates a term-document matrix for [LAYER] based on [META]";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 2)
        return;

      var vm = new TermDocumentMatrixViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        MetadataKey = args[2]
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}