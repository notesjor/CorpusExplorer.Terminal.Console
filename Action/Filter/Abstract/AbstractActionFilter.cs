using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action.Filter.Abstract
{
  public abstract class AbstractActionFilter
  {
    public abstract HashSet<string> Operator { get; }
    public abstract string Request { get; }
    public abstract Selection Execute(Selection selection, string @operator, string target, string query);
  }
}