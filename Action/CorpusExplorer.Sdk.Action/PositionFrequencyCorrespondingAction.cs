using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class PositionFrequencyCorrespondingAction : IAction
  {
    public string Action => "position-frequency-corresponding";

    public string Description => "position-frequency [LAYER1] [WORD1] [LAYER2] [WORDS2] - left/right position of words around [WORD1] in [LAYER1] + corresponding [LAYER2] [WORDS2] filter.";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var arguments = args.ToList();
      var l1 = arguments[0];
      arguments.RemoveAt(0);
      var w1 = arguments[0];
      arguments.RemoveAt(0);
      var l2 = arguments[0];
      arguments.RemoveAt(0);

      var vm = new PositionFrequencyViewModel
      {
        Selection = selection,
        LayerDisplayname = l1,
        LayerQueries = new[] { w1 },
        CorrespondingLayerValueFilter = new CorrespondingLayerValueFilterViewModel
        {
          Layer1Displayname = l1,
          Layer2Displayname = l2,
          AnyMatch = true,
          Layer2ValueFilters = new HashSet<string>(arguments),
          Selection = selection
        }
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}