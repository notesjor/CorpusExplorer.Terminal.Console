using System;
using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Filter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action.Filter
{
  public class ActionFilterFulltextExactPhrase : AbstractActionFilter
  {
    public override HashSet<string> Operator => new HashSet<string> {"phrase", "!phrase"};
    public override string Request => "text";

    public override Selection Execute(Selection selection, string @operator, string target, string query)
    {
      return selection.CreateTemporary(
        new AbstractFilterQuery[]
        {
          new FilterQuerySingleLayerExactPhrase
          {
            Inverse = @operator.StartsWith("!"),
            LayerDisplayname = target,
            LayerQueries = query.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries)
          }
        });
    }
  }
}