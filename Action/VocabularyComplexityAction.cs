using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class VocabularyComplexityAction : AbstractAction
  {
    public override string Action => "vocabulary-complexity";
    public override string Description => "vocabulary-complexity [LAYER] - vocabulary complexity in [LAYER]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new VocabularyComplexityViewModel {Selection = selection};
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Execute();
      var table = vm.GetDataTable();

      writer.WriteTable(selection.Displayname, table);
    }
  }
}