using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicSignificantFilterAction : IAction
  {
    public string Action => "kwic-sig";

    public string Description => Resources.DescKwicSig;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 3)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);
      queries.RemoveAt(0);

      TextLiveSignificanceSearchViewModel vm;
      if (args[1] == "y" || args[1] == "Y")
        vm = new TextLiveSignificanceSearchViewModel
        {
          Selection = selection,
          HighlightBodyStart = "<div>",
          HighlightBodyEnd = "</div>"
        };
      else
        vm = new TextLiveSignificanceSearchViewModel
        {
          Selection = selection,
          HighlightBodyStart = "",
          HighlightBodyEnd = "",
          HighlightStart = "",
          HighlightEnd = ""
        };
      vm.AddQuery(new FilterQuerySingleLayerAnyMatch { LayerDisplayname = args[0], LayerQueries = queries });
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}