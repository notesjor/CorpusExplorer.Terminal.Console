using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class SentenceCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "how-many-sentences" };

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput(selection.CountSentences.ToString());
    }
  }
}