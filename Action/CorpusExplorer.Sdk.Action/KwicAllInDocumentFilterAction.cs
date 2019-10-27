using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicAllInDocumentFilterAction : AbstractFilterAction
  {
    public override string Action => "kwic-document";

    public override string Description => Resources.DescKwicAllInDocument;

    protected override AbstractFilterQuery GetQuery(string layerDisplayname, IEnumerable<string> queries) 
      => new FilterQuerySingleLayerAllInOneDocument {LayerDisplayname = layerDisplayname, LayerQueries = queries};
  }
}