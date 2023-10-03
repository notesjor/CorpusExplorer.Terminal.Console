using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaUniqueSelectAction : IAction
  {
    public string Action => "meta-unique-select";
    public string Description => Resources.DescMetaUniqueSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length == 0)
        return;

      var meta = selection.DocumentMetadata;

      var res = new Dictionary<string, HashSet<string>>();
      foreach (var m in new HashSet<string>(args))
        res.Add(m, new HashSet<string>());

      foreach (var d in meta)
        foreach (var x in d.Value)
        {
          if (res.ContainsKey(x.Key))
            res[x.Key].Add(x.Value?.ToString());
        }

      var dt = new DataTable();
      dt.Columns.Add(Resources.Category, typeof(string));
      dt.Columns.Add(Resources.Frequency, typeof(int));

      dt.BeginLoadData();
      foreach (var x in res)
        dt.Rows.Add(x.Key, x.Value.Count);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}
