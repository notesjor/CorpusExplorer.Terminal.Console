using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency2Action : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> {"freq2", "frequency2"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var block = selection.CreateBlock<Frequency2LayerBlock>();
      block.Calculate();

      WriteOutput("pos\tterm\tfrequency\r\n");
      foreach (var x in block.Frequency)
      foreach (var y in x.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{y.Value}\r\n");
    }
  }
}