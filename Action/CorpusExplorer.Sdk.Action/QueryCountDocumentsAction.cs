using System;
using System.Data;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public class QueryCountDocumentsAction : IAction
  {
    public string Action => "query-count-documents";
    public string Description => Resources.DescQueryCountDocuments;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 1)
        return;

      var dt = new DataTable();
      dt.Columns.Add(Resources.Query, typeof(string));
      dt.Columns.Add(Resources.Frequency, typeof(double));

      dt.BeginLoadData();
      if (args[0].StartsWith("FILE:"))
      {
        var lines = File.ReadAllLines(args[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        foreach(var x in lines.ToDictionary(x => x, QueryParser.Parse))
          dt.Rows.Add(x.Key, QuickQuery.CountOnDocumentLevel(selection, new[] { x.Value }));
      }
      else
      {
        var s = args[0].Split(Splitter.ColonColon, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length != 2)
          return;

        var query = QueryParser.Parse(args[0]);
        if (query is FilterQueryUnsupportedParserFeature feature)
        {
          UnsupportedQueryParserFeatureHelper.Handle(selection, feature, args[1], writer);
          return;
        }

        dt.Rows.Add(args[0], QuickQuery.CountOnDocumentLevel(selection, new[] { query }));
      }

      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}