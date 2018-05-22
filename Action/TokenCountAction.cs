using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class TokenCountAction : AbstractAction
  {
    public override string Action => "how-many-tokens";
    public override string Description => "how-many-tokens - sum of all tokens";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("param", typeof(string));
      dt.Columns.Add("value", typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add("tokens", (double) selection.CountToken);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}