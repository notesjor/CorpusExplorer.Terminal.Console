using System.Collections.Generic;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicAnyFilterAction : AbstractFilterAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "kwic-any" };

    protected override AbstractFilterQuery GetQuery() => new FilterQuerySingleLayerAnyMatch();
  }
}