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
  public class MetaUniqueValuesAction : IAction
  {
    public string Action => "meta-unique-values";
    public string Description => Resources.DescMetaUniqueValues;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var meta = selection.DocumentMetadata;

      var res = new Dictionary<string, Dictionary<string, int>>();
      foreach (var d in meta)
        foreach (var x in d.Value)
        {
          if (!res.ContainsKey(x.Key))
            res.Add(x.Key, new Dictionary<string, int>());
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
        foreach (var y in x.Value)
          dt.Rows.Add(x.Key, y.Key, y.Value);
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }
  }
}

