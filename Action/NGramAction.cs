using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class NGramAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"ngram", "n-gram"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var size = int.Parse(args.Last());

      var block = selection.CreateBlock<Ngram1LayerBlock>();
      block.NGramSize = size;
      block.Calculate();

      WriteOutput("ngram\tfrequency\r\n");
      foreach (var x in block.NGramFrequency)
        WriteOutput($"{x.Key}\t{x.Value}\r\n");
    }
  }
}