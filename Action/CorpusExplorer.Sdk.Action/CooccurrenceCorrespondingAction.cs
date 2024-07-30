using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceCorrespondingAction : IAction
  {
    public string Action => "cooccurrence-corresponding";

    public string Description => Resources.DescCooccurrenceCorresponding;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new CooccurrenceViewModel { Selection = selection };

      var arguments = args.ToList();
      var l1 = arguments[0];
      arguments.RemoveAt(0);
      var l2 = arguments[0];
      arguments.RemoveAt(0);

      vm.LayerDisplayname = l1;

      var corresponding = new CorrespondingLayerValueFilterViewModel
      {
        Layer1Displayname = l1,
        Layer2Displayname = l2,
        AnyMatch = true,
        Layer2ValueFilters = new HashSet<string>(arguments),
        Selection = selection
      };
      corresponding.Execute();

      vm.CorrespondingLayerValueFilter = corresponding;
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetFullDataTable());
    }
  }
}