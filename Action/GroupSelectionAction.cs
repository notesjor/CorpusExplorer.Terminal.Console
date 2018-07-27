using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class GroupSelectionAction : AbstractAction
  {
    internal Dictionary<string, AbstractAction> _actions;
    public override string Action => "group";
    public override string Description => "group - similar to [cluster] (only available in CEScripts - allows cluster analytics over queries";
    
    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      // NICHT VERFÜGBAR
    }

    public void Execute(List<Selection> selections, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 1)
        return;

      var nargs = new List<string>(args);
      var task = nargs[0];
      if (!_actions.ContainsKey(task))
        return;
      nargs.RemoveAt(0);

      var dt = new DataTable();
      dt.Columns.Add("CLUSTER", typeof(string));
      
      foreach (var s in selections)
      {
        var bypass = new BypassTableWriter();
        _actions[task].Execute(s, nargs.ToArray(), bypass);
        DataTableMergerHelper.MergeDataTables(s.Displayname, ref dt, ref bypass);
      }

      writer.WriteTable(dt);
    }
  }
}