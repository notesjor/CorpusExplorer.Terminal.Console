using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class KwicAllInDocumentFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-document";

    public override string Description =>
      "kwic-document [LAYER] [TEXT] - [TEXT] = space separated tokens - a document must contains all token";

    protected override AbstractFilterQuery GetQuery()
    {
      return new FilterQuerySingleLayerAllInOnDocument();
    }
  }
}