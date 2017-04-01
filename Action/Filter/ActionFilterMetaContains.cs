using System;
using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Filter.Abstract;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Port.RProgramming.Api.Action.Filter
{
  public class ActionFilterMetaContains : AbstractActionFilter
  {
    public override HashSet<string> Operator => new HashSet<string> {"contains", "!contains"};
    public override string Request => "meta";

    public override Selection Execute(Selection selection, string @operator, string target, string query)
    {
      return selection.CreateTemporary(
        new AbstractFilterQuery[]
        {
          new FilterQueryMetaContains
          {
            Inverse = @operator.StartsWith("!"),
            MetaLabel = target,
            MetaValues = query.Split(new[] {"|", ",", ";"}, StringSplitOptions.RemoveEmptyEntries)
          }
        });
    }
  }
}