using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class SentenceCountAction : AbstractAction
  {
    public override string Action => "how-many-sentences";
    public override string Description => "how-many-sentences - sum of all sentences";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("param", typeof(string));
      dt.Columns.Add("value", typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add("sentences", (double) selection.CountSentences);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}