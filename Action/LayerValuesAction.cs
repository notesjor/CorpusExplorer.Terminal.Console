using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class LayerValuesAction : AbstractAction
  {
    public override string Action => "get-types";
    public override string Description => "get-types [LAYER] - list all [LAYER]-values (types)";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length == 0)
        return;

      var dt = new DataTable();
      dt.Columns.Add("type", typeof(string));

      var values = new HashSet<string>(selection.GetLayers(args[0]).SelectMany(layer => layer.Values));
      dt.BeginLoadData();
      foreach (var value in values)
        dt.Rows.Add(value);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}