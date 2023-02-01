using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class ValidateAction : IAction
  {
    public string Action => "validate";
    public string Description => Resources.DescValidate;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new ValidateSelectionIntegrityViewModel { Selection = selection };
      vm.Execute();
      writer.WriteTable(vm.GetDataTable());
    }
  }
}
