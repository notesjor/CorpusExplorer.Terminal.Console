using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
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