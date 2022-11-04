using System.Linq;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicExportAction : AbstractActionWithExport
  {
    public override string Action => "kwic-export";
    public override string Description => Resources.DescKwicExport;

    protected override AbstractCorpusAdapter ExecuteCall(Selection selection, string[] args, string path)
    {
      if (args == null || args.Length < 2)
        return null;

      var layer = args[0];
      var queries = FileQueriesHelper.ResolveFileQueries(args[1].Replace("FILE:", ""));
      var spanHelper = new KwicSpanHelper(queries);
      var merger = new CorpusMergerQueryBasedKwic
      {
        AddContextSentencesPre = spanHelper.SentencePre,
        AddContextSentencesPost = spanHelper.SentencePost,
        FilterQueries = new AbstractFilterQuery[]
        {
          new FilterQuerySingleLayerAnyMatch
          {
            LayerDisplayname = layer,
            LayerQueries = spanHelper.CleanArguments
          }
        }
      };
      merger.Input(selection.ToCorpus());
      merger.Execute();

      return merger.Output.First();
    }
  }
}