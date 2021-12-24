using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class NGramCorrespondingAction : IAction
  {
    public string Action => "ngram-corresponding";
    public string Description => Resources.DescNGramCorresponding;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var arguments = args.ToList();
      var n = int.Parse(arguments[0]);
      arguments.RemoveAt(0);
      var l1 = arguments[0];
      arguments.RemoveAt(0);
      var min = int.Parse(arguments[0]);
      arguments.RemoveAt(0);
      var l2 = arguments[0];
      arguments.RemoveAt(0);
      var any = bool.Parse(arguments[0]);
      arguments.RemoveAt(0);

      var vm = new NgramViewModel
      {
        Selection = selection,
        LayerDisplayname = l1,
        NGramMinFrequency = min,
        NGramSize = n,
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