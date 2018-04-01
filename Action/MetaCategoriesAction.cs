using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaCategoriesAction : AbstractAction
  {
    public override string Action => "meta-categories";
    public override string Description => "meta-categories - all available names for meta categories";

    public override void Execute(Selection selection, string[] args)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties();

      var dt = new DataTable();
      dt.Columns.Add("meta-categories", typeof(string));

      dt.BeginLoadData();
      foreach (var x in categories)
        dt.Rows.Add(x);
      dt.EndLoadData();

      WriteTable(dt);
    }
  }
}