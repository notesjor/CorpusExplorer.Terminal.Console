using System.Collections.Generic;

namespace CorpusExplorer.Terminal.Console.Web.Model
{
  public class ActionFilter
  {
    private readonly HashSet<string> _filter;
    private readonly bool _mode;

    public ActionFilter(bool allow, params string[] actions)
    {
      _mode = allow;
      _filter = new HashSet<string>(actions);
    }

    public bool Check(string action)
    {
      return _filter.Contains(action) ? _mode : !_mode;
    }
  }
}