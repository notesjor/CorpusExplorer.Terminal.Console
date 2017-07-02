using System.Collections.Generic;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency3Action : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> {"freq3", "frequency3"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var block = selection.CreateBlock<Frequency3LayerBlock>();
      block.Calculate();

      WriteOutput("pos\tlemma\tterm\tfrequency\r\n");
      foreach (var x in block.Frequency)
      foreach (var y in x.Value)
      foreach (var z in y.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{z.Key}\t{z.Value}\r\n");
    }
  }
}