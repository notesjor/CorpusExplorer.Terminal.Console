using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class ClusterListAction : IAction
  {
    public string Action => "cluster-list";

    public string Description => Resources.DescClusterList;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 1)
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;

      var selections =
        UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature) query);
      if (selections == null)
        return;

      foreach (var sub in selections)
        writer.WriteTable(sub.Displayname, sub.GetDocumentGuidAndDisplaynamesAsDataTable());
    }
  }
}