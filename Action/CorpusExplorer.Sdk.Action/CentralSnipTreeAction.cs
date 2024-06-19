﻿using CorpusExplorer.Sdk.Action.Helper;
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

    public string Description => "central-snip-tree [LAYER1] [PRE] [LAYER2] [POST] [WORD/FILE] - generates a digraph of matching [LAYER1][WORDS/FILE] based on [LAYER2] with [PRE] and [POST] fixes";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 5)
        return;

      var block = selection.CreateBlock<CentralSnipBlock>();
      block.Layer1Displayname = args[0];
      block.NPre = int.Parse(args[1]);
      block.Layer2Displayname = args[2];
      block.NPost = int.Parse(args[3]);

      var queries = args.Skip(4).ToList();
      block.LayerQueries = FileQueriesHelper.ResolveFileQueries(queries);

      block.Calculate();

      writer.WriteDirectThroughStream(ConvertToDigraph(string.Join(" ", block.LayerQueries), block.FrequencyPre, block.FrequencyPost));
    }

    private string ConvertToDigraph(string root, Dictionary<string, int> pre, Dictionary<string, int> post)
    {
      var stb = new StringBuilder();
      stb.AppendLine("digraph G {");

      var a = pre.Values.Max();
      var b = post.Values.Max();
      var max = a > b ? a : b;

      var pNodes = new Dictionary<int, Dictionary<string, string>>();

      foreach (var x in pre)
      {
        var parts = x.Key.Split(' ');
        string last = "";

        var p = (double)x.Value / max * 25d;
        if (p < 1)
          p = 1;
        var w = (int)p;

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = (parts.Length - i) * -1;
          string part = parts[i];

          if (!pNodes.ContainsKey(idx))
            pNodes.Add(idx, new Dictionary<string, string>());

          string current;
          if (pNodes[idx].ContainsKey(part))
            current = pNodes[idx][part];
          else
          {
            current = $"p{idx:D2}_{pNodes[idx].Count:D5}";
            pNodes[idx].Add(part, current);
          }          

          if (i > 0)
            stb.AppendLine($"\t\"{last}\" -> \"{current}\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
          last = current;
        }

        stb.AppendLine($"\t\"{last}\" -> \"main\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
      }

      foreach (var x in post)
      {
        var parts = x.Key.Split(' ');

        var p = (double)x.Value / max * 25d;
        if (p < 1)
          p = 1;
        var w = (int)p;

        string last = "main";

        for (var i = 0; i < parts.Length; i++)
        {
          var idx = i + 1;
          string part = parts[i];

          if (!pNodes.ContainsKey(idx))
            pNodes.Add(idx, new Dictionary<string, string>());

          string current;
          if (pNodes[idx].ContainsKey(part))
            current = pNodes[idx][part];
          else
          {
            current = $"p{idx:D2}_{pNodes[idx].Count:D5}";
            pNodes[idx].Add(part, current);
          }          

          stb.AppendLine($"\t\"{last}\" -> \"{current}\" [ label=\"{x.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
          last = current;
        }
      }

      stb.AppendLine();

      stb.AppendLine($"main [ label=\"{Filter(root)}\"] weight=0 ]");

      foreach (var p in pNodes)
        foreach (var x in p.Value)
          stb.AppendLine($"{x.Value} [ label=\"{Filter(x.Key)}\" weight={p.Key} ]");

      stb.AppendLine("}");

      return stb.ToString();
    }

    private static string Filter(object content)
    {
      return content.ToString().Replace("\"", "''");
    }
  }
}
