using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class TypeCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "how-many-types" };

    public override void Execute(Selection selection, string[] args)
    {
      if (args == null || args.Length == 0)
        WriteOutput("0");

      WriteOutput(new HashSet<string>(selection.GetLayers(args[0]).SelectMany(layer => layer.Values)).Count.ToString());
    }
  }
}