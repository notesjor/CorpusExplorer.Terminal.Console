using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action.Abstract
{
  public abstract class AbstractFilterAction : IAction
  {
    public abstract string Action { get; }
    public abstract string Description { get; }

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);

      queries = ResolveFileQueriesRecursive(queries);

      var vm = new TextLiveSearchViewModel { Selection = selection };
      if (queries.Any(query => query.Contains(" ")))
        foreach (var query in queries)
        {
          vm.AddQuery(GetQuery(args[0], query.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)));
        }
      else
        vm.AddQuery(GetQuery(args[0], queries));
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetUniqueDataTableCsv());
    }

    private List<string> ResolveFileQueriesRecursive(List<string> queries)
    {
      var res = new List<string>();
      foreach (var query in queries)
      {
        if (query.StartsWith("FILE:"))
          res.AddRange(ResolveFileQueriesRecursive(query));
        else
          res.Add(query);
      }

      return res;
    }

    private List<string> ResolveFileQueriesRecursive(string path)
    {
      var res = new List<string>();
      foreach (var query in File.ReadAllLines(path.Replace("FILE:", ""), Configuration.Encoding))
      {
        if (query.StartsWith("FILE:"))
          res.AddRange(ResolveFileQueriesRecursive(query));
        else
          res.Add(query);
      }

      return res;
    }

    protected abstract AbstractFilterQuery GetQuery(string layerDisplayname, IEnumerable<string> queries);
  }
}