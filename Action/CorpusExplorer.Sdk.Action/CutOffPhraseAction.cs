using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class CutOffPhraseAction : IAction
  {
    public string Action => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if(args.Length != 3)
        return;

      var vm = new CutOffPhraseViewModel { Selection = selection };
      vm.LayerDisplayname = args[0];
      vm.LayerQuery1 = args[1];
      vm.LayerQuery2 = args[2];
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
