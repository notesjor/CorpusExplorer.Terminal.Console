using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class SkipgramAction : IAction
  {
    public string Action => "skipgram";
    public string Description => "skipgram [LAYER] - calculates skipgrams for [LAYER]";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 1)
        return;

      var vm = new NormalizedSkipgramProbabilityViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}