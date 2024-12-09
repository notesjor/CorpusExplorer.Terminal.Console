using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Blocks.Range;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class CrossFrequencySelectRangeAction : IAction
  {
    public string Action => "cross-frequency-select-range";
    public string Description => "cross-frequency-select-range [LAYER] [WORDS] [FROM] [TO] - calculates the cross-frequency for [WORDS] based on [LAYER] in range (FROM/TO)";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var block = selection.CreateBlock<CrossFrequencySelectedRangeBlock>();
      block.LayerDisplayname = args[0];
      block.Ranges = new RangeSimple(int.Parse(args[1]), int.Parse(args[2]));
      block.LayerQueries = args.Skip(3).ToArray();
      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add($"{args[0]} (A)");
      dt.Columns.Add($"{args[0]} (B)");
      dt.Columns.Add("Frequency");

      dt.BeginLoadData();
      foreach (var x in block.CooccurrencesFrequency)
      foreach (var y in x.Value)
        dt.Rows.Add(x.Key, y.Key, y.Value);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}