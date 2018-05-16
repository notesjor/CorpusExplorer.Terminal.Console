using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicAnyFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-any";

    public override string Description =>
      "kwic-any [LAYER] [TEXT] - KWIC any occurrence - [TEXT] = space separated tokens";

    protected override AbstractFilterQuery GetQuery()
    {
      return new FilterQuerySingleLayerAnyMatch();
    }
  }
}