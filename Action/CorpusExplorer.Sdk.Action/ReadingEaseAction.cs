using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Extern.NHunspell.ViewModel;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class ReadingEaseAction : IAction
  {
    public string Action => "reading-ease";
    public string Description => "reading-ease [LAYER] - calculates reading ease metrics based on [LAYER]";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new ReadingEaseViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0]
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}