using System;
using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class ClusterAction : AbstractAction
  {
    internal Dictionary<string, AbstractAction> _actions;
    public override string Action => "cluster";
    public override string Description => "cluster [QUERY] [TASK] [ARGUMENTS] - executes a [TASK] for every [CLUSTER]s";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 2 || args[1].ToLower() == "cluster")
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;      

      var selections = UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature) query);
      if (selections == null)
        return;

      var nargs = new List<string>(args);
      nargs.RemoveAt(0);
      var task = nargs[0];
      if (!_actions.ContainsKey(task))
        return;

      var dt = new DataTable();
      dt.Columns.Add("CLUSTER", typeof(string));

      nargs.RemoveAt(0);
      foreach (var s in selections)
      {
        var bypass = new BypassTableWriter();
        _actions[task].Execute(s, nargs.ToArray(), bypass);
        MergeDataTables(s.Displayname, ref dt, ref bypass);
      }

      writer.WriteTable(dt);
    }

    private void MergeDataTables(string cluster, ref DataTable dt, ref BypassTableWriter bypass)
    {
      foreach (DataColumn column in bypass.Table.Columns)
        if (!dt.Columns.Contains(column.ColumnName))
          dt.Columns.Add(column.ColumnName, column.DataType);
      
      dt.BeginLoadData();

      foreach (DataRow row in bypass.Table.Rows)
      {
        var items = new List<object> {cluster};
        items.AddRange(row.ItemArray);
        dt.Rows.Add(items.ToArray());
      }

      dt.EndLoadData();
    }
  }
}