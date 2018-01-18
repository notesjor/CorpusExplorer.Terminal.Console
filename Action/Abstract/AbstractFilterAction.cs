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
        case AbstractFilterQuerySingleLayer _:
          ((AbstractFilterQuerySingleLayer)query).LayerDisplayname = args[0];
          ((AbstractFilterQuerySingleLayer)query).LayerQueries = queries;
          break;
        case FilterQuerySingleLayerExactPhrase _:
          ((FilterQuerySingleLayerExactPhrase)query).LayerDisplayname = args[0];
          ((FilterQuerySingleLayerExactPhrase)query).LayerQueries = queries;
          break;
      }


      var vm = new TextLiveSearchViewModel { Selection = selection };
      vm.AddQuery(query);
      vm.Analyse();

      var data = vm.GetUniqueData();

      WriteOutput("pre\tmatch\tpost\tfreq\tdocId\tsentId\r\n");
      foreach (var x in data)
      foreach (var s in x.Sentences)
        WriteOutput($"{x.Pre}\t{x.Match}\t{x.Post}\t{x.Count}\t{s.Key:N}\t{s.Value}\r\n");
    }

    protected abstract AbstractFilterQuery GetQuery();
  }
}