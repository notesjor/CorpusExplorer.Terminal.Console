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
  public class MetaUniqueAction : IAction
  {
    public string Action => "meta-unique";
    public string Description => Resources.DescMetaUnique;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var meta = selection.DocumentMetadata;

      var res = new Dictionary<string, HashSet<string>>();
      foreach (var d in meta)
        foreach (var x in d.Value)
        {
          if (!res.ContainsKey(x.Key))
            res.Add(x.Key, new HashSet<string>());
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
