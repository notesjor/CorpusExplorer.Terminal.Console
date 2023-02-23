using CorpusExplorer.Sdk.Addon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class PhraseCountAction : IAction
  {
    public string Action => "phrase-count";
    public string Description => Resources.DescPhraseCount;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var arguments = new List<string>(args);
      arguments.RemoveAt(0);

      PhraseFastCounterViewModel vm = new PhraseFastCounterViewModel
      {
        Selection = selection,
        AutoSplitLayerQuieresWithSpace = false,
        LayerDisplayname = args[0],
        LayerQueries = FileQueriesHelper.ResolveFileQueries(arguments).Select(x => x.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }

    protected IEnumerable<FilterQuerySingleLayerExactPhrase> GetQuery(string layerDisplayname, IEnumerable<string> queries)
      => queries.Select(q => new FilterQuerySingleLayerExactPhrase
      {
        LayerDisplayname = layerDisplayname,
        LayerQueries = q.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)
      });
  }
}
