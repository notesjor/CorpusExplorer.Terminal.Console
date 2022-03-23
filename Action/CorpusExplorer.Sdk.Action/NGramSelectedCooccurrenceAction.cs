using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class NGramSelectedCooccurrenceAction : IAction
  {
    public string Action => "ngram-select-sig";

    public string Description => "ngram-select-sig [N] [LAYER] [WORDS/FILE] - all [N]-grams on [LAYER] containing [WORDS] or FILE:[FILE] rated by [WORD/FILE] co-occurrences.";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 3)
        return;

      var block = selection.CreateBlock<Ngram1LayerSelectiveHighlightCooccurrencesBlock>();
      block.NGramSize = int.Parse(args[0]);
      block.LayerDisplayname = args[1];

      if (args.Length == 3 && args[2].StartsWith("FILE:"))
        block.LayerQueries = File.ReadAllLines(args[2].Replace("FILE:", ""), Configuration.Encoding);
      else
      {
        var lst = args.ToList();
        lst.RemoveAt(0);
        lst.RemoveAt(0);
        block.LayerQueries = lst;
      }

      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add(Resources.NGram, typeof(string));

      var max = block.NgramsWeighted[0].Key.Length;
      for(var cnt = 1; cnt <= max; cnt++)
      {
        dt.Columns.Add($"{Resources.Token} ({cnt})", typeof(string));
        dt.Columns.Add($"{Resources.Rank} ({cnt})", typeof(byte));
      }
      dt.Columns.Add(Resources.Frequency, typeof(double));
      dt.Columns.Add(Resources.Significance + " (max.)", typeof(double));
      dt.Columns.Add(Resources.Significance + " (sum)", typeof(double));

      dt.BeginLoadData();
      foreach (var x in block.NgramsWeighted)
      {
        var data = new List<object> { string.Join(" ", x.Key.Select(y => y.Key)) };
        foreach (var y in x.Key)
        {
          data.Add(y.Key);
          data.Add(y.Value);
        }

        data.Add(x.Value[0]);
        data.Add(x.Value[1]);
        data.Add(x.Value[2]);
        dt.Rows.Add(data.ToArray());
      }
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}