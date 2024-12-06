using System.Collections.Generic;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Helper;

namespace CorpusExplorer.Sdk.Action
{
  public class CrossFrequencySelectAction : IAction
  {
    public string Action => "cross-frequency-select";
    public string Description => "cross-frequency-select [LAYER] [WORDS] - calculates the cross-frequency for [WORDS] based on [LAYER]";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 2)
        return;

      var queries = new HashSet<string>(args.Skip(1));

      var layerName = args[0];
      var block = selection.CreateBlock<CrossFrequencySelectedBlock>();
      block.LayerDisplayname = layerName;
      block.LayerQueries = queries;
      block.Calculate();

      var fdic = block.CooccurrencesFrequency.CompleteDictionaryToFullDictionary();

      var dt = new DataTable();
      dt.Columns.Add("Query", typeof(string));
      dt.Columns.Add(layerName, typeof(string));
      dt.Columns.Add("Frequency", typeof(double));
      dt.BeginLoadData();
      foreach (var q in queries)
        if (fdic.ContainsKey(q))
          foreach (var x in fdic[q])
            dt.Rows.Add(q, x.Key, x.Value);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}