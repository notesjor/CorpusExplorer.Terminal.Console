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
  public class MetaUniqueSelectValuesAction : IAction
  {
    public string Action => "meta-unique-select-values";
    public string Description => Resources.DescMetaUniqueSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length == 0)
        return;

      var meta = selection.DocumentMetadata;

      var res = new Dictionary<string, Dictionary<string, int>>();
      foreach (var m in new HashSet<string>(args))
        res.Add(m, new Dictionary<string, int>());

      foreach (var d in meta)
        foreach (var x in d.Value)
        {
          if (!res.ContainsKey(x.Key))
            continue;

          var yKey = x.Value?.ToString();
          if (!res[x.Key].ContainsKey(yKey))
            res[x.Key].Add(yKey, 0);
          res[x.Key][yKey]++;
        }

      var dt = new DataTable();
      dt.Columns.Add(Resources.Category, typeof(string));
      dt.Columns.Add(CorpusExplorer.Sdk.Properties.Resources.MetadataLabel, typeof(string));
      dt.Columns.Add(Resources.Frequency, typeof(int));

      dt.BeginLoadData();
      foreach (var x in res)
        foreach(var y in x.Value)
          dt.Rows.Add(x.Key, y.Key, y.Value);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}
