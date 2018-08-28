using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class GetDocumentDisplaynamesAction : IAddonConsoleAction
  {
    public string Action => "get-document-displaynames";
    public string Description => "get-document-displaynames - get all document GUID / display-names.";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dnames = selection.DocumentGuidsAndDisplaynames;

      var dt = new DataTable();
      dt.Columns.Add("GUID", typeof(string));
      dt.Columns.Add("display-name", typeof(string));

      dt.BeginLoadData();
      foreach (var pair in dnames)
        dt.Rows.Add(pair.Key.ToString("N"), pair.Value);

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}