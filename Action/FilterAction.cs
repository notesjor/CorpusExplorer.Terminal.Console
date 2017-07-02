using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Action.Filter;
using CorpusExplorer.Terminal.Console.Action.Filter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : AbstractAction
  {
    private static readonly AbstractActionFilter[] _filter =
    {
      new ActionFilterFulltextAny(),
      new ActionFilterFulltextInDocument(),
      new ActionFilterFulltextExactPhrase(),
      new ActionFilterFulltextInSentence(),
      new ActionFilterFulltextRegex(),
      new ActionFilterMetaContains(),
      new ActionFilterMetaRegex()
    };

    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"query", "filter", "select", "snapshot"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var a = args?.ToArray();
      if (a == null || a.Length != 5)
        return;

      var request = a[0].ToLower();
      var target = a[1];
      var @operator = a[2].ToLower();
      var query = a[3];
      var output = a[4];

      var sub = (from filter in _filter
                 where filter.Request == request && filter.Operator.Contains(@operator)
                 select filter.Execute(selection, @operator, target, query)).FirstOrDefault();
      if (sub == null || sub.CountToken == 0)
        return;

      var export = new OutputAction();
      export.Execute(sub, new[] {output});
    }
  }
}