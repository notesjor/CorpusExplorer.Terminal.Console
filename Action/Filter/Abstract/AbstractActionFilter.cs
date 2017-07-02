using System.Collections.Generic;

namespace CorpusExplorer.Terminal.Console.Action.Filter.Abstract
{
  public abstract class AbstractActionFilter
  {
    public abstract HashSet<string> Operator { get; }
    public abstract string Request { get; }
    public abstract Selection Execute(Selection selection, string @operator, string target, string query);
  }
}