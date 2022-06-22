using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicAnyFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-any";

    public override string Description => Resources.DescKwicAny;

    protected override AbstractFilterQuery GetQuery(string layerDisplayname, IEnumerable<string> queries)
      => new FilterQuerySingleLayerAnyMatch { LayerDisplayname = layerDisplayname, LayerQueries = queries };
  }
}