using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KeywordAction : IAction
  {
    public string Action => "keyword";
    public string Description => "keyword [LAYER] [TSV_RefFile] [COL_Token] [COL_RelFreq] - calculates the keynes of any [LAYER]-value by using a reference list [TSV_RefFile].";
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
