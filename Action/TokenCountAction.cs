using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class TokenCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"many-token"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      WriteOutput(selection.CountToken.ToString());
    }
  }
}