using CorpusExplorer.Sdk.Addon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceDiversityAction : IAction
  {
    public string Action => "cooccurrence-diversity";
    public string Description => "cooccurrence-diversity [LAYER] - calculate the diversity of cooccurrences for a given [LAYER]";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 1)
        return;

      var vm = new CooccurrenceDiversityViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0]
      };

      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
