using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency2Action : AbstractAction
  {
    public override string Action => "frequency2";
    public override string Description => "frequency2 [LAYER1] [LAYER2] - count token frequency on 2 layers";

    public override void Execute(Selection selection, string[] args)
    {
      var block = selection.CreateBlock<Frequency2LayerBlock>();
      if (args != null && args.Length == 2)
      {
        block.Layer1Displayname = args[0];
        block.Layer2Displayname = args[1];
      }
      block.Calculate();

      WriteOutput("pos\tterm\tfrequency\r\n");
      foreach (var x in block.Frequency)
        foreach (var y in x.Value)
          WriteOutput($"{x.Key}\t{y.Key}\t{y.Value}\r\n");
    }
  }
}