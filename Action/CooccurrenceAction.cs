using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class CooccurrenceAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> {"cooc", "cooccurrence"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var block = selection.CreateBlock<CooccurrenceBlock>();
      block.Calculate();

      WriteOutput("termA\ttermB\tsignificance\r\n");
      foreach (var x in block.CooccurrenceSignificance)
      foreach (var y in x.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{y.Value}\r\n");
    }
  }
}