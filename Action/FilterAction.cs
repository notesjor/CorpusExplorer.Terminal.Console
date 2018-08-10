using System;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Helper;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : IAddonConsoleAction
  {
    public string Action => "query";
    public string Description => "query - see help section [OUTPUT] for more information";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 2)
        return;

      Selection sub;
      if (args[0].StartsWith("FILE:"))
      {
        var lines = File.ReadAllLines(args[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        sub = queries.Aggregate(selection,
                                (current, q) => current.Create(new[] {q}, Path.GetFileNameWithoutExtension(args[1])));
      }
      else
      {
        var s = args[0].Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length != 2)
          return;

        var query = QueryParser.Parse(args[0]);
        if (query is FilterQueryUnsupportedParserFeature)
        {
          UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature) query, args[1],
                                                     writer);
          return;
        }

        sub = selection.Create(new[] {query}, Path.GetFileNameWithoutExtension(args[1]));
      }

      var export = new OutputAction();
      export.Execute(sub, new[] {args[1]}, writer);
    }
  }
}