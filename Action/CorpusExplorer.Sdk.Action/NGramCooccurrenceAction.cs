using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
namespace CorpusExplorer.Sdk.Action
{
  public class NGramCooccurrenceAction : IAction
  {
    public string Action => "ngram-sig";
    public string Description => "ngram-sig [N] [LAYER] - [N] sized N-gram based on [LAYER] rated with co-occurence-levels";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 2)
        return;

      var block = selection.CreateBlock<NgramHighlightCooccurrencesBlock>();
      block.NGramSize = int.Parse(args[0]);
      block.LayerDisplayname = args[1];
      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add(Resources.NGram, typeof(string));

      var max = block.WeightedNgrams.Keys.Max(x => x.Length);
      for (var cnt = 1; cnt <= max; cnt++)
      {
        dt.Columns.Add($"{Resources.Token} ({cnt})", typeof(string));
        dt.Columns.Add($"{Resources.Rank} ({cnt})", typeof(byte));
      }
      dt.Columns.Add(Resources.Frequency, typeof(double));
      dt.Columns.Add(Resources.Significance, typeof(double));

      var len = dt.Columns.Count - 2;

      dt.BeginLoadData();
      foreach (var x in block.WeightedNgrams)
      {
        var data = new List<object> { string.Join(" ", x.Key.Select(y => y.Key)) };
        foreach (var y in x.Key)
        {
          data.Add(y.Key);
          data.Add(y.Value);
        }

        while (data.Count < len)
        {
          data.Add("");
          data.Add(0);
        }

        data.Add(x.Value[0]);
        data.Add(x.Value[1]);
        dt.Rows.Add(data.ToArray());
      }
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}