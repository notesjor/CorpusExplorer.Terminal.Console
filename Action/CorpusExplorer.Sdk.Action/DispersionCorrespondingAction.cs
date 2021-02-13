using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DispersionCorrespondingAction : IAction
  {
    public string Action => "dispersion-corresponding";
    public string Description => "dispersion [LAYER1] [META] [LAYER2] [ANY] [WORDS] - calculates dispersions values of all [LAYER1] values based on [META] and annply correspondign filter.";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var arguments = args.ToList();
      var l1 = arguments[0];
      arguments.RemoveAt(0);
      var meta = arguments[0];
      arguments.RemoveAt(0);
      var l2 = arguments[0];
      arguments.RemoveAt(0);
      var any = bool.Parse(arguments[0]);
      arguments.RemoveAt(0);

      var vm = new DispersionViewModel
      {
        Selection = selection,
        CorrespondingLayerValueFilter = new CorrespondingLayerValueFilterViewModel
        {
          Layer1Displayname = l1,
          Layer2Displayname = l2,
          AnyMatch = any,
          Layer2ValueFilters = new HashSet<string>(arguments),
          Selection = selection
        },
        LayerDisplayname = l1,
        MetadataKey = meta
      };

      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}