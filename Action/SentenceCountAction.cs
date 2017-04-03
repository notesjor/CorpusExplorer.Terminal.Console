using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class SentenceCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"many-sentence"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      WriteOutput(selection.CountSentences.ToString());
    }
  }
}