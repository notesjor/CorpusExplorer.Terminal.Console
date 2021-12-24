using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CrossFrequencyCorrespondingAction : IAction
  {
    public string Action => "cross-frequency-corresponding";
    public string Description => Resources.DescCrossFrequencyCorresponding;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var arguments = args.ToList();
      var l1 = arguments[0];
      arguments.RemoveAt(0);
      var l2 = arguments[0];
      arguments.RemoveAt(0);
      var any = bool.Parse(arguments[0]);
      arguments.RemoveAt(0);

      var vm = new FrequencyCrossViewModel
      {
        Selection = selection,
        LayerDisplayname = l1,
        CorrespondingLayerValueFilter = new CorrespondingLayerValueFilterViewModel
        {
          Layer1Displayname = l1,
          Layer2Displayname = l2,
          AnyMatch = any,
          Layer2ValueFilters = new HashSet<string>(arguments),
          Selection = selection
        }
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}