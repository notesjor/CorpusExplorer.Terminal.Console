using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class TokenCountAction : IAction
  {
    public string Action => "how-many-tokens";
    public string Description => Resources.DescHowManyToken;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Param, typeof(string));
      dt.Columns.Add(Resources.Value, typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add(Resources.Tokens, (double) selection.CountToken);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}