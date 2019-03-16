using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class SentenceCountAction : IAction
  {
    public string Action => "how-many-sentences";
    public string Description => Resources.DescHowManySentences;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Param, typeof(string));
      dt.Columns.Add(Resources.Value, typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add(Resources.Sentences, (double) selection.CountSentences);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}