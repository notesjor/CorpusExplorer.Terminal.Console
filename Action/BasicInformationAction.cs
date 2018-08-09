using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class BasicInformationAction : IAddonConsoleAction
  {
    public string Action => "basic-information";
    public string Description => "basic-information - basic information tokens/sentences/documents";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add("param", typeof(string));
      dt.Columns.Add("value", typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add("tokens", (double) selection.CountToken);
      dt.Rows.Add("tokens-factor", 1000000.0 / selection.CountToken);
      dt.Rows.Add("sentences", (double) selection.CountSentences);
      dt.Rows.Add("documents", (double) selection.CountDocuments);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}