using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class CorrespondingValuesAction : IAction
  {
    public string Action => "corresponding";
    public string Description => Resources.CorrespondingValuesActionDescription;
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 2)
        return;
      
      var block = selection.CreateBlock<CorrespondingLayerValueBlock>();
      block.Layer1Displayname = args[0];
      block.Layer2Displayname = args[1];
      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add(args[0], typeof(string));
      dt.Columns.Add(args[1], typeof(string));

      dt.BeginLoadData();
      foreach (var x in block.CorrespondingLayerValues)
        dt.Rows.Add(x.Key, string.Join(" | ", x.Value));
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}
