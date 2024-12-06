using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System.Data;

namespace CorpusExplorer.Sdk.Action
{
  public class Frequency1PerSentenceAction : IAction
  {
    public string Action => "frequency1-per-sentence";
    public string Description => "frequency1-per-sentence {LAYER} - count token frequency on {LAYER} (default: Wort) per sentence";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var layerName = args != null && args.Length == 1 ? args[0] : "Wort";
      var block = selection.CreateBlock<Frequency1LayerOneOccurrencePerSentenceBlock>();
      block.LayerDisplayname = layerName;
      block.Calculate();

      var dt = new DataTable();
      dt.Columns.Add(layerName, typeof(string));
      dt.Columns.Add("Frequency", typeof(double));
      dt.BeginLoadData();
      foreach(var x in block.Frequency)
        dt.Rows.Add(x.Key, x.Value);
      dt.EndLoadData();
      
      writer.WriteTable(selection.Displayname, dt);
    }
  }
}