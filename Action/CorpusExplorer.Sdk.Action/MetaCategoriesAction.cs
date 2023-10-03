using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaCategoriesAction : IAction
  {
    public string Action => "meta-categories";
    public string Description => Resources.DescMetaCategories;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties();

      var dt = new DataTable();
      dt.Columns.Add(Resources.Category, typeof(string));

      dt.BeginLoadData();
      foreach (var x in categories)
        dt.Rows.Add(x);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}