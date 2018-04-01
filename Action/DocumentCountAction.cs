using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class DocumentCountAction : AbstractAction
  {
    public override string Action => "how-many-documents";
    public override string Description => "how-many-documents - sum of all documents";

    public override void Execute(Selection selection, string[] args)
    {
      var dt = new DataTable();
      dt.Columns.Add("param", typeof(string));
      dt.Columns.Add("value", typeof(double));

      dt.BeginLoadData();
      dt.Rows.Add("documents", (double)selection.CountDocuments);
      dt.EndLoadData();

      WriteTable(dt);
    }
  }
}