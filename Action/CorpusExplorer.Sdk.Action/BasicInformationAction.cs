using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class BasicInformationAction : IAction
  {
    public string Action => "basic-information";
    public string Description => Resources.DescBasicInformation;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var token = (double)selection.CountToken;
      var documents = (double)selection.CountDocuments;
      var sentences = (double)selection.CountSentences;

      var dt = new DataTable();
      dt.Columns.Add(Resources.Param, typeof(string));
      dt.Columns.Add(Resources.Value, typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add(Resources.Tokens, token);
      dt.Rows.Add(Resources.TokenFactor, 1000000.0 / token);
      dt.Rows.Add(Resources.Sentences, sentences);
      dt.Rows.Add(Resources.Documents, documents);

      try
      {
        var types = selection.GetLayerValues("Wort").Count();
        dt.Rows.Add(Resources.Types, types);
        dt.Rows.Add(Resources.TTR, types / token);
        dt.Rows.Add(Resources.TSR, types / sentences);
        dt.Rows.Add(Resources.TDR, types / documents);
      }
      catch
      {
        // ignore
      }

      dt.Rows.Add(Resources.ToSR, token / sentences);
      dt.Rows.Add(Resources.ToDR, token / documents);
      dt.Rows.Add(Resources.SDR, sentences / documents);

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}