using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class TokenListSelectAction : IAction
  {
    public string Action => "token-list-select";
    public string Description => Resources.DescTokeListSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Tokens, typeof(string));

      var regex = new Regex(args[1], RegexOptions.Compiled);

      dt.BeginLoadData();
      foreach (var v in selection.GetLayers(args[0]).FirstOrDefault().Values)
        if(regex.IsMatch(v))
          dt.Rows.Add(v);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}