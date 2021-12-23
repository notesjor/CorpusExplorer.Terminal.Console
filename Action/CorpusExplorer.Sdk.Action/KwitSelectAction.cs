﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KwitSelectAction : IAction
  {
    public string Action => "kwit-n";

    public string Description => Resources.DescKwitSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 6)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      var pre = int.Parse(queries[0]);
      queries.RemoveAt(0);
      var post = int.Parse(queries[0]);
      queries.RemoveAt(0);

      var vm = new TextFlowSearchWithRangeSelectionViewModel
      {
        Selection = selection,
        Layer1Displayname = args[0],
        Layer2Displayname = args[1],
        MinFrequency = int.Parse(args[2]),
        LayerQueryPhrase = queries,
        AutoJoin = true,
        HighlightCooccurrences = false,
        Pre = pre,
        Post = post
      };
      vm.Execute();

      writer.WriteDirectThroughStream(Convert(vm.DiscoveredConnections.ToArray()));
    }

    private string Convert(Tuple<string, int, string>[] connections)
    {
      if (connections == null || connections.Length == 0)
        return string.Empty;

      var stb = new StringBuilder();
      stb.Append("digraph G {\r\n");

      var max = connections.Select(x => x.Item2).Max();

      foreach (var connection in connections)
      {
        if (connection.Item2 == 0)
          continue;

        var a = Filter(connection.Item1);
        var b = Filter(connection.Item3);
        var p = (double)connection.Item2 / max * 25d;
        if (p < 1)
          p = 1;
        var w = (int)p;

        stb.AppendLine($"\t\"{a}\" -> \"{b}\" [ label=\"{connection.Item2}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
      }

      stb.Append("\r\n}\r\n");
      return stb.ToString();
    }

    private string Filter(object content)
    {
      return content.ToString().Replace("\"", "''");
    }
  }
}