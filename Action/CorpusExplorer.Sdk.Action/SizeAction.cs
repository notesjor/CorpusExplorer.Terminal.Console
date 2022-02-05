using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class SizeAction : IAction
  {
    public string Action => "size";
    public string Description => "size - returns the size (sentences/tokens) of all documents";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Guid, typeof(string));
      dt.Columns.Add(Resources.Sentences, typeof(int));
      dt.Columns.Add(Resources.Tokens, typeof(int));

      var first = selection.Layers.First();
      var @lock = new object();

      dt.BeginLoadData();
      Parallel.ForEach(selection.DocumentGuids, Configuration.ParallelOptions, dsel =>
      {
        try
        {
          if(!first.ContainsDocument(dsel))
            return;
          var doc = first[dsel];

          var s = doc.Length;
          var t = doc.Sum(x => x.Length);

          lock (@lock)
            dt.Rows.Add(dsel.ToString("N"), s, t);
        }
        catch
        {
          // ignore
        }
      });
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}
