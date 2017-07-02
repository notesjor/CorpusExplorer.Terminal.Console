using System.Collections.Generic;
using CorpusExplorer.Terminal.Console.Action.Filter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action.Filter
{
  public class ActionFilterMetaRegex : AbstractActionFilter
  {
    public override HashSet<string> Operator => new HashSet<string> {"regex"};
    public override string Request => "meta";

    public override Selection Execute(Selection selection, string @operator, string target, string query)
    {
      return selection.CreateTemporary(
        new AbstractFilterQuery[]
        {
          new FilterQueryMetaRegex
          {
            Inverse = @operator.StartsWith("!"),
            MetaLabel = target,
            MetaValues = new object[] {query}
          }
        });
    }
  }
}