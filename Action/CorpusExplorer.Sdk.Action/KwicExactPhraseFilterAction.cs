using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicExactPhraseFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-phrase";

    public override string Description => Resources.DescKwicPhrase;

    protected override IEnumerable<AbstractFilterQuery> GetQuery(string layerDisplayname, IEnumerable<string> queries)
      => queries.Select(q => new FilterQuerySingleLayerExactPhrase { LayerDisplayname = layerDisplayname, LayerQueries = q.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries) });
  }
}