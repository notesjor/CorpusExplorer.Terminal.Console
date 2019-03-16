using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class TypeCountAction : IAction
  {
    public string Action => "how-many-types";
    public string Description => Resources.DescHowManyTypes;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length == 0)
        return;

      var dt = new DataTable();
      dt.Columns.Add(Resources.Param, typeof(string));
      dt.Columns.Add(Resources.Value, typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add(Resources.Types,
                  (double) new HashSet<string>(selection.GetLayers(args[0]).SelectMany(layer => layer.Values)).Count);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}