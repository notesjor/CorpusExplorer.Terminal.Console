using System.Collections.Generic;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicAllInSentenceFilterAction : AbstractFilterAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "kwic-sentence" };

    protected override AbstractFilterQuery GetQuery() => new FilterQuerySingleLayerAllInOneSentence();
  }
}