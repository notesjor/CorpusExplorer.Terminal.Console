using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class SentenceCountAction : AbstractAction
  {
    public override string Action => "how-many-sentences";
    public override string Description => "how-many-sentences - sum of all sentences";

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput(selection.CountSentences.ToString());
    }
  }
}