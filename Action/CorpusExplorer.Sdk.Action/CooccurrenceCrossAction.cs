using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceCrossAction : IAction
  {
    public string Action => "cooccurrence-cross";

    public string Description => Resources.DescCooccurrencesCross;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var list = new List<string>(args);
      list.RemoveAt(0);

      var vm = new CooccurrenceCrossViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        LayerValues = list,
        EnableExternalCooccurrencesFilter = true
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}