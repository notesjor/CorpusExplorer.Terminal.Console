using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class CrossFrequencyAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> {"cfreq", "cross-frequency", "crossfrequency" };

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var block = selection.CreateBlock<CrossFrequencyBlock>();
      block.Calculate();

      WriteOutput("termA\ttermB\tsignificance\r\n");
      foreach (var x in block.CooccurrencesFrequency)
      foreach (var y in x.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{y.Value}\r\n");
    }
  }
}