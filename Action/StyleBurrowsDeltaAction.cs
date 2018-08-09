using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class StyleBurrowsDeltaAction : IAddonConsoleAction
  {
    public string Action => "style-burrowsd";

    public string Description =>
      "style-burrowsd [META1] [META2] - compares [META1] with [META2] based on \"Burrows Delta\"";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 2)
        return;
      
      var vm = new ClusterMetadataByBurrowsDeltaViewModel
      {
        Selection = selection,
        MetadataKeyA = args[0],
        MetadataKeyB = args[1]
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetCrossDataTable());
    }
  }
}