using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class TokenListAction : IAction
  {
    public string Action => "token-list";
    public string Description => Resources.DescTokeList;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Tokens, typeof(string));

      dt.BeginLoadData();
      foreach (var v in selection.GetLayers(args[0]).FirstOrDefault().Values)
        dt.Rows.Add(v);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}