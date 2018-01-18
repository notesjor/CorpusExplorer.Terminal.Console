using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class LayerValuesAction : AbstractAction
  {
    public override string Action => "get-types";
    public override string Description => "get-types [LAYER] - list all [LAYER]-values (types)";

    public override void Execute(Selection selection, string[] args)
    {
      if (args == null || args.Length == 0)
        WriteOutput("");

      WriteOutput("type\r\n");
      var values = new HashSet<string>(selection.GetLayers(args[0]).SelectMany(layer => layer.Values));
      foreach (var value in values)
      {
        WriteOutput($"{value}\r\n");
      }
    }
  }
}