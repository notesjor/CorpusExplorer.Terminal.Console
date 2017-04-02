using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class NGramAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "ngram", "n-gram" };

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var size = (from x in args where x.StartsWith("n:") select int.Parse(x.Replace("n:", ""))).FirstOrDefault();
      if (size < 1)
        size = 2;

      var block = selection.CreateBlock<NgramPatternBlock>();
      block.NGramSizeMin = size;
      block.NGramSizeMax = size;
      block.Calculate();

      WriteOutput("ngram\tfrequency\r\n");
      foreach (var x in block.NGramSimpleFrequency)
        WriteOutput($"{x.Key}\t{x.Value}\r\n");
    }
  }
}