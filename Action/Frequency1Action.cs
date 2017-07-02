using System.Collections.Generic;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency1Action : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"freq", "frequency", "freq1", "frequency1"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var block = selection.CreateBlock<Frequency1LayerBlock>();
      block.Calculate();

      WriteOutput("term\tfrequency\r\n");
      foreach (var x in block.Frequency)
        WriteOutput($"{x.Key}\t{x.Value}\r\n");
    }
  }
}