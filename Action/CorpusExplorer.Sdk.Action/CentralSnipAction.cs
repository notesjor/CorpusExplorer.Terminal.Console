using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class CentralSnipAction : IAction
  {
    public string Action => "central-snip";

    public string Description => "central-snip [LAYER1] [PRE] [LAYER2] [POST] [WORDS/FILE] - find all central snips for [LAYER1]/[WORDS/FILE] based on [LAYER2]";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 5)
        return;

      var block = selection.CreateBlock<CentralSnipBlock>();
      block.Layer1Displayname = args[0];
      block.NPre = int.Parse(args[1]);
      block.Layer2Displayname = args[2];
      block.NPost = int.Parse(args[3]);

      var queries = args.Skip(4).ToList();
      block.LayerQueries = FileQueriesHelper.ResolveFileQueries(queries);

      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add("Position", typeof(int));
      dt.Columns.Add(args[2], typeof(string));
      dt.Columns.Add("Frequency", typeof(int));

      dt.BeginLoadData();
      foreach(var x in block.FrequencyPre)
        dt.Rows.Add(-1, x.Key, x.Value);
      foreach (var x in block.FrequencyPost)
        dt.Rows.Add(1, x.Key, x.Value);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}
