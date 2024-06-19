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
  public class Phrase2LayerCountAction : IAction
  {
    public string Action => "phrase-2layer-count";

    public string Description => "phrase-2layer-count [LAYER1] [LAYER2] [WORDS1-PHRASE] - Search a Phrase [WORDS1-PHRASE] in [LAYER1] and sum up the frequency by [LAYER2]";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if(args.Length < 3)
        return;

      var block = selection.CreateBlock<Frequency2LayerSelectBlock>();
      block.Layer1Displayname = args[0];
      block.Layer2Displayname = args[1];
      block.Layer1Queries = args.Skip(2);
      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add("Matches", typeof(string));
      dt.Columns.Add("Frequency", typeof(double));

      dt.BeginLoadData();
      foreach(var kvp in block.Frequency)
        dt.Rows.Add(kvp.Key, kvp.Value);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}
