using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class CentralSnipTreeAction : IAction
  {
    public string Action => "central-snip-tree";

    public string Description => "central-snip-tree [minFreq] [LAYER1] [PRE] [LAYER2] [POST] [WORDS/FILE] - generates a digraph of matching [LAYER1][WORDS/FILE] based on [LAYER2] with [PRE] and [POST] fixes (filter by [minFreq])";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 6)
        return;

      int minFreq = int.Parse(args[0]);

      var block = selection.CreateBlock<CentralSnipBlock>();
      block.Layer1Displayname = args[1];
      block.NPre = int.Parse(args[2]);
      block.Layer2Displayname = args[3];
      block.NPost = int.Parse(args[4]);

      var queries = args.Skip(5).ToList();
      block.LayerQueries = FileQueriesHelper.ResolveFileQueries(queries);

      block.Calculate();

      var root = Filter(string.Join(" ", block.LayerQueries));

      writer.WriteDirectThroughStream(ConvertToDigraph(root, minFreq, block.FrequencyPre, block.FrequencyPost));
    }

    private string ConvertToDigraph(string root, int minFreq, Dictionary<string, int> pre, Dictionary<string, int> post)
    {
      var stb = new StringBuilder();
      stb.AppendLine("digraph G {");

      var a = pre.Values.Max();
      var b = post.Values.Max();
      var max = a > b ? a : b;

      CalculatePositionFrequency(pre, post, out var filter);

      foreach (var x in pre)
      {
        var parts = x.Key.Split(' ');
        string last = null;

        var p = (double)x.Value / max * 25d;
        if (p < 1)
          p = 1;
        var w = (int)p;

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = (parts.Length - i) * -1;
          string part = parts[i];

          if (filter[idx][part] < minFreq)
            continue;

          var current = Filter(part);

          if (!string.IsNullOrWhiteSpace(last))
            stb.AppendLine($"\t\"{last}\" -> \"{current}\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");

          last = current;
        }

        if (!string.IsNullOrWhiteSpace(last))
          stb.AppendLine($"\t\"{last}\" -> \"{root}\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
      }

      foreach (var x in post)
      {
        var parts = x.Key.Split(' ');

        var p = (double)x.Value / max * 25d;
        if (p < 1)
          p = 1;
        var w = (int)p;

        string last = root;

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = i + 1;
          string part = parts[i];

          if (filter[idx][part] < minFreq)
            continue;

          var current = Filter(part);

          if (!string.IsNullOrWhiteSpace(last))
            stb.AppendLine($"\t\"{last}\" -> \"{current}\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
          
          last = current;
        }
      }

      stb.AppendLine();

      stb.AppendLine("}");

      return stb.ToString();
    }

    private void CalculatePositionFrequency(Dictionary<string, int> pre, Dictionary<string, int> post, out Dictionary<int, Dictionary<string, int>> filter)
    {
      filter = new Dictionary<int, Dictionary<string, int>>();

      foreach (var x in pre)
      {
        var parts = x.Key.Split(' ');

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = (parts.Length - i) * -1;
          string part = parts[i];

          if (!filter.ContainsKey(idx))
            filter.Add(idx, new Dictionary<string, int>());

          if (filter[idx].ContainsKey(part))
            filter[idx][part] += x.Value;
          else
            filter[idx].Add(part, x.Value);
        }
      }

      foreach (var x in post)
      {
        var parts = x.Key.Split(' ');

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = i + 1;
          string part = parts[i];

          if (!filter.ContainsKey(idx))
            filter.Add(idx, new Dictionary<string, int>());

          if (filter[idx].ContainsKey(part))
            filter[idx][part] += x.Value;
          else
            filter[idx].Add(part, x.Value);
        }
      }
    }

    private static string Filter(object content)
    {
      return content.ToString().Replace("\"", "''");
    }
  }
}
