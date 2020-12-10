using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class VocabularyComplexityAction : IAction
  {
    public string Action => "vocabulary-complexity";
    public string Description => Resources.DescVocabularyComplexity;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new VocabularyComplexityViewModel { Selection = selection };
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTableSimple());
    }
  }
}