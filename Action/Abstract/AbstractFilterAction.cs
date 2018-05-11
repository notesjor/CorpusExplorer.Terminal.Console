using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Terminal.Console.Action.Abstract
{
  public abstract class AbstractFilterAction : AbstractAction
  {
    public override void Execute(Selection selection, string[] args)
    {
      if (args == null || args.Length < 2)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);

      var query = GetQuery();
      switch (query)
      {
        case AbstractFilterQuerySingleLayer q:
          q.LayerDisplayname = args[0];
          q.LayerQueries = queries;
          break;
        case FilterQuerySingleLayerExactPhrase q:
          q.LayerDisplayname = args[0];
          q.LayerQueries = queries;
          break;
      }
      
      var vm = new TextLiveSearchViewModel { Selection = selection };
      vm.AddQuery(query);
      vm.Analyse();

      WriteTable(vm.GetDataTable());
    }

    protected abstract AbstractFilterQuery GetQuery();
  }
}