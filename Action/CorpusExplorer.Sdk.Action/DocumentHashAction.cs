using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentHashAction : IAction
  {
    public string Action => "hash";

    public string Description => "hash [LAYER] [ALGO] - calculates a hashsum for all documents in [LAYER]. [ALGO] = MD5, SHA1, SHA256, SHA512";
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
