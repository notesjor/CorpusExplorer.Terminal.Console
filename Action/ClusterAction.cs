using System;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class ClusterAction : AbstractAction
  {
    public override string Action => "cluster";
    public override string Description => "cluster [QUERY] [TASK] [ARGUMENTS] - executes a [TASK] for every [CLUSTER]s";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var query = QueryParser.Parse(args[0]);
      if (query is FilterQueryUnsupportedParserFeature)
      {
        var s = args[0].Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length != 2)
          return;

        UnsupportedParserFeatureHandler(selection, (FilterQueryUnsupportedParserFeature) query, args[1], writer);
      }
    }
  }
}