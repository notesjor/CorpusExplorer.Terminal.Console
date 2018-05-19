using System.Collections.Generic;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicFirstAnyFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-first-any";

    public override string Description =>
      "kwic-first-any [LAYER] [WORDS] - KWIC any occurrence - [WORDS] = space separated tokens (KWIC must contains first token + any other)";

    protected override AbstractFilterQuery GetQuery(string layerDisplayname, IEnumerable<string> queries)
    {
      return new FilterQuerySingleLayerFirstAndAnyOtherMatch{LayerDisplayname = layerDisplayname, LayerQueries = queries};
    }
  }
}