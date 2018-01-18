using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class LayerNamesAction : AbstractAction
  {
    public override string Action => "layer-names";
    public override string Description => "layer-names - all available names for [LAYER]";

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput("layernames\r\n");
      foreach (var x in selection.LayerDisplaynames)
        WriteOutput($"{x}\r\n");
    }
  }
}