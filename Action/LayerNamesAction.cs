using System.Collections.Generic;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class LayerNamesAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"layer-names", "layernames"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      WriteOutput("layernames\r\n");
      foreach (var x in selection.LayerDisplaynames)
        WriteOutput($"{x}\r\n");
    }
  }
}