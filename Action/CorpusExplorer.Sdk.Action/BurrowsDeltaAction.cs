using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class BurrowsDeltaAction : IAction
  {
    public string Action => "burrows-delta";

    public string Description =>
      "burrows-delta {META} {SIZE} - calculate burrows delta for [META] (default: Autor) with [SIZE] samples (default: 2000)";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new BurrowsDeltaViewModel
      {
        Selection = selection,
        MetadataKey = args.Length > 0 ? args[0] : "Autor",
        MFWCount = args.Length > 2 ? int.Parse(args[1]) : 2000
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
