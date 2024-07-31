using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceSelectedCorrespondingAction : IAction
  {
    public string Action => "cooccurrence-select-corresponding";

    public string Description => Resources.DescCooccurrenceSelectedCorresponding;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 4)
        return;

      var arguments = args.ToList();
      var layer1 = arguments[0];
      arguments.RemoveAt(0);
      var layer1value = arguments[0];
      arguments.RemoveAt(0);
      var layer2 = arguments[0];
      arguments.RemoveAt(0);
      var layer2values = new HashSet<string>(arguments);

      var coresponding = new CorrespondingLayerValueFilterViewModel
      {
        Layer1Displayname = layer1,
        Layer2Displayname = layer2,
        AnyMatch = true,
        Layer2ValueFilters = layer2values,
        Selection = selection
      };
      coresponding.Execute();

      var vm = new CooccurrenceSelectiveViewModel
      {
        Selection = selection,
        LayerDisplayname = layer1,
        LayerQueries = new[] { layer1value },
        CorrespondingLayerValueFilter = coresponding
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}