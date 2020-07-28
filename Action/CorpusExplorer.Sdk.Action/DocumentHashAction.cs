using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentHashAction : IAction
  {
    public string Action => "hash";

    public string Description => Resources.DescHash;
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DocumentHashViewModel {Selection = selection};
      if (args.Length >= 1)
        vm.LayerDisplayname = args[0];
      if (args.Length == 2)
        vm.LayerDisplayname = args[1];
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
