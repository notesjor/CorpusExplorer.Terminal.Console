using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency2Action : AbstractAction
  {
    public override string Action => "frequency2";
    public override string Description => "frequency2 [LAYER1] [LAYER2] - count token frequency on 2 layers";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency2LayerViewModel {Selection = selection};
      if (args != null && args.Length == 2)
      {
        vm.Layer1Displayname = args[0];
        vm.Layer2Displayname = args[1];
      }

      vm.Analyse();
      writer.WriteTable(vm.GetDataTable());
    }
  }
}