using System;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class QueryAction : AbstractActionWithExport
  {
    public override string Action => "query";
    public override string Description => Resources.DescQuery;

    protected override AbstractCorpusAdapter ExecuteCall(Selection selection, string[] args, string path)
    {
      if (args == null || args.Length < 1)
        return null;

      Selection res;
      if (args[0].StartsWith("FILE:"))
      {
        var lines = File.ReadAllLines(args[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        res = queries.Aggregate(selection,
          (current, q) => current.Create(new[] {q}, Path.GetFileNameWithoutExtension(path), false));
      }
      else
      {
        var s = args[0].Split(Splitter.ColonColon, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length != 2)
          return null;

        var query = QueryParser.Parse(args[0]);
        if (query is FilterQueryUnsupportedParserFeature feature)
        {
          // UnsupportedQueryParserFeatureHelper.Handle(selection, feature, args[1], writer); - Macht hier keinen Sinn
          return null;
        }

        res = selection.Create(new[] {query}, Path.GetFileNameWithoutExtension(path), false);
      }
      return res.ToCorpus();
    }
  }
}