using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class BasicInformationAction : AbstractAction
  {
    public override string Action => "basic-information";
    public override string Description => "basic-information - basic information tokens/sentences/documents";

    public override void Execute(Selection selection, string[] args)
    {
      var dt = new DataTable();
      dt.Columns.Add("param", typeof(string));
      dt.Columns.Add("value", typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add("tokens", (double)selection.CountToken);
      dt.Rows.Add("tokens-factor", 1000000.0 / selection.CountToken);
      dt.Rows.Add("sentences", (double)selection.CountSentences);
      dt.Rows.Add("documents", (double)selection.CountDocuments);
      dt.EndLoadData();

      WriteTable(dt);
    }
  }
}