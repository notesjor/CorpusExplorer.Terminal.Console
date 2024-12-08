using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Blocks.Range;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CooccurrenceClassicAction : IAction
  {
    public string Action => "cooccurrence-classic [LAYER] [FROM] [TO]";
    public string Description { get; }
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 4)
        return;

      var block = selection.CreateBlock<CooccurrenceClassicBlock>();
      block.LayerDisplayname = args[0];
      var from = int.Parse(args[1]);
      var to = int.Parse(args[2]);
      block.Ranges = new RangeSimple(from, to);
      block.LayerQueries = args.Skip(3).ToArray();

      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add(args[0], typeof(string));
      dt.Columns.Add("Cooccurrence", typeof(string));
      dt.Columns.Add("Frequency", typeof(int));
      dt.Columns.Add("Significance", typeof(double));

      dt.BeginLoadData();
      foreach (var x in block.CooccurrenceSignificance)
        foreach (var y in x.Value)
          dt.Rows.Add(x.Key, y.Key, block.CooccurrenceFrequency[x.Key][y.Key], y.Value);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}
