using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class GetDocumentDisplaynamesAction : IAction
  {
    public string Action => "get-document-displaynames";
    public string Description => Resources.DescGetDocumentDisplayname;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      writer.WriteTable(selection.Displayname, selection.GetDocumentGuidAndDisplaynamesAsDataTable());
    }
  }
}