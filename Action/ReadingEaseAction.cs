using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class ReadingEaseAction : AbstractAction
  {
    public override string Action => "reading-ease";
    public override string Description => "reading-ease [LAYER] - reading ease of [LAYER]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new ReadingEaseViewModel { Selection = selection };
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Analyse();
      var table = vm.GetDataTable();

      writer.WriteTable(table);
    }
  }
}