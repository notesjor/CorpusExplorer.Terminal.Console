using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class TokenCountAction : AbstractAction
  {
    public override string Action => "how-many-tokens";
    public override string Description => "how-many-tokens - sum of all tokens";

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput(selection.CountToken.ToString());
    }
  }
}