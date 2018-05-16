using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicAllInSentenceFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-sentence";

    public override string Description =>
      "kwic-sentence [LAYER] [TEXT] - [TEXT] = space separated tokens - a sentence must contains all token";

    protected override AbstractFilterQuery GetQuery()
    {
      return new FilterQuerySingleLayerAllInOneSentence();
    }
  }
}