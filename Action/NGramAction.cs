using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class NGramAction : AbstractAction
  {
    public override string Action => "n-gram";
    public override string Description => "n-gram [N] [LAYER] - [N] sized N-gram based on [LAYER]";

    public override void Execute(Selection selection, string[] args)
    {
      var block = selection.CreateBlock<Ngram1LayerBlock>();
      if (args == null || args.Length == 0)
        block.NGramSize = 5;
      if (args.Length >= 1)
        block.NGramSize = int.Parse(args[0]);
      if (args.Length == 2)
        block.LayerDisplayname = args[1];
      block.Calculate();

      WriteOutput("ngram\tfrequency\r\n");
      foreach (var x in block.NGramFrequency)
        WriteOutput($"{x.Key}\t{x.Value}\r\n");
    }
  }
}