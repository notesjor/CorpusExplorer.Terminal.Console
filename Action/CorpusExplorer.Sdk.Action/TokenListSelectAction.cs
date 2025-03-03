using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Sdk.Action
{
  public class TokenListSelectAction : IAction
  {
    public string Action => "token-list-select";
    public string Description => Resources.DescTokeListSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var dt = new DataTable();
      dt.Columns.Add(Resources.Tokens, typeof(string));

      if (args[1].StartsWith("FILE:"))
        dt = ExecuteFile(selection, args, dt);
      else
        dt = ExecuteRegex(selection, args, dt);
      writer.WriteTable(selection.Displayname, dt);
    }

    private DataTable ExecuteFile(Selection selection, string[] args, DataTable dt)
    {
      var lines = File.ReadAllLines(args[1].Substring(5), Configuration.Encoding);
      var values = new HashSet<string>(selection.GetLayers(args[0]).FirstOrDefault().Values);

      foreach (var q in lines)
        if (q.StartsWith("*"))
          foreach (var c in values.Where(v => v.EndsWith(q.Substring(1))))
            dt.Rows.Add(c);
        else if (q.EndsWith("*"))
          foreach (var c in values.Where(v => v.StartsWith(q.Substring(0, q.Length - 1))))
            dt.Rows.Add(c);
        else
          if (values.Contains(q))
          dt.Rows.Add(q);
      
      return dt;
    }

    private static DataTable ExecuteRegex(Selection selection, string[] args, DataTable dt)
    {
      var regex = new Regex(args[1], RegexOptions.Compiled);

      dt.BeginLoadData();
      foreach (var v in selection.GetLayers(args[0]).FirstOrDefault().Values)
        if (regex.IsMatch(v))
          dt.Rows.Add(v);
      dt.EndLoadData();

      return dt;
    }
  }
}