using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KeywordAction : IAction
  {
    public string Action => "keyword";
    public string Description => Resources.keyword_desc;
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      try
      {
        if (args.Length != 4)
          return;

        var vm = new KeywordPresetReferenceListViewModel { Selection = selection, LayerDisplayname = args[0] };
        vm.LoadRefList(args[1], int.Parse(args[2]), int.Parse(args[3]));
        vm.Execute();

        writer.WriteTable(vm.GetDataTable());
      }
      catch
      {
        // ignore
      }
    }
  }
}
