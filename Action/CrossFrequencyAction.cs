using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class CrossFrequencyAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "cross-frequency" };

    public override void Execute(Selection selection, string[] args)
    {
      var block = selection.CreateBlock<CrossFrequencyBlock>();
      if (args != null && args.Length == 1)
        block.LayerDisplayname = args[0];
      block.Calculate();

      WriteOutput("termA\ttermB\tsignificance\r\n");
      foreach (var x in block.CooccurrencesFrequency)
      foreach (var y in x.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{y.Value}\r\n");
    }
  }
}