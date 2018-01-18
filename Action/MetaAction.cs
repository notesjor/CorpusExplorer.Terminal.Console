using System.Collections.Generic;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> { "meta" };

    public override void Execute(Selection selection, string[] args)
    {
      var block = selection.CreateBlock<DocumentMetadataWeightBlock>();
      if (args != null && args.Length == 1)
        block.LayerDisplayname = args[0];
      block.Calculate();

      WriteOutput("category\tlabel\ttokens\ttypes\tdocuments\r\n");
      foreach (var x in block.GetAggregatedSize())
      foreach (var y in x.Value)
        WriteOutput($"{x.Key}\t{y.Key}\t{y.Value[0]}\t{y.Value[1]}\t{y.Value[2]}\r\n");
    }
  }
}