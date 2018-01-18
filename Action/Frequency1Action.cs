using System;
using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency1Action : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "frequency1" };

    public override void Execute(Selection selection, string[] args)
    {
      var block = selection.CreateBlock<Frequency1LayerBlock>();
      if (args != null && args.Length == 1)
        block.LayerDisplayname = args[0];
      block.Calculate();

      WriteOutput("term\tfrequency\r\n");
      foreach (var x in block.Frequency)
        WriteOutput($"{x.Key}\t{x.Value}\r\n");
    }
  }
}