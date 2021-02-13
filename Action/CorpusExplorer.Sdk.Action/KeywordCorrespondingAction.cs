using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KeywordCorrespondingAction : IAction
  {
    public string Action => "keyword-corresponding";
    public string Description => "keyword [LAYER1] [TSV_RefFile] [COL_Token] [COL_RelFreq] [LAYER2] [WORDS2] - calculates keyword (see [ACTION = keyword]) and applies the corresponding filter.";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      try
      {
        var arguments = args.ToList();
        var l1 = arguments[0];
        arguments.RemoveAt(0);
        var rFile = arguments[0];
        arguments.RemoveAt(0);
        var colToken = arguments[0];
        arguments.RemoveAt(0);
        var colFreq = arguments[0];
        arguments.RemoveAt(0);
        var l2 = arguments[0];
        arguments.RemoveAt(0);

        var vm = new KeywordPresetReferenceListViewModel
        {
          Selection = selection, 
          LayerDisplayname = l1,
          CorrespondingLayerValueFilter = new CorrespondingLayerValueFilterViewModel
          {
            Layer1Displayname = l1,
            Layer2Displayname = l2,
            AnyMatch = true,
            Layer2ValueFilters = new HashSet<string>(arguments),
            Selection = selection
          }
        };
        vm.LoadRefList(rFile, int.Parse(colToken), int.Parse(colFreq));
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