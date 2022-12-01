using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicAllInSentenceFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-sentence";

    public override string Description => Resources.DescKwicAllInSentence;

    protected override IEnumerable<AbstractFilterQuery> GetQuery(string layerDisplayname, IEnumerable<string> queries)
      => new[] { new FilterQuerySingleLayerAllInOneSentence { LayerDisplayname = layerDisplayname, LayerQueries = queries } };
  }
}