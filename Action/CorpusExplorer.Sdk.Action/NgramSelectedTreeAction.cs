using CorpusExplorer.Sdk.Addon;
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
  public class NgramSelectedTreeAction : IAction
  {
    public string Action => "ngram-select-tree";

    public string Description => "";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 4)
        return;

      var vm = new Ngram1LayerSelectiveViewModel
      {
        Selection = selection,
        NGramSize = int.Parse(args[0]),
        LayerDisplayname = args[1],
        NGramMinFrequency = int.Parse(args[2])
      };

      var queries = args.ToList();
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);

      if (queries.Count == 1 && queries[0].StartsWith("FILE:"))
        vm.LayerQueries = File.ReadAllLines(queries[0].Replace("FILE:", ""), Configuration.Encoding);
      else
        vm.LayerQueries = queries;

      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraph(vm.NGramFrequency));
    }

    private string ConvertToDigraph(Dictionary<string, double> nGramFrequency)
    {
      var stb = new StringBuilder();
      stb.AppendLine("digraph G {");

      var max = nGramFrequency.Select(x => x.Value).Max();
      var pNodes = new Dictionary<int, Dictionary<string, string>>();

      foreach (var n in nGramFrequency)
      {
        var parts = n.Key.Split(' ');
        string last = "";

        for (int i = 0; i < parts.Length; i++)
        {
          string part = parts[i];
          if (!pNodes.ContainsKey(i))
            pNodes.Add(i, new Dictionary<string, string>());

          string current;
          if (pNodes[i].ContainsKey(part))
            current = pNodes[i][part];
          else
          {
            current = $"p{i:D2}_{pNodes[i].Count:D5}";
            pNodes[i].Add(part, current);
          }

          var p = n.Value / max * 25d;
          if (p < 1)
            p = 1;
          var w = (int)p;

          if (i > 0)
            stb.AppendLine($"\t\"{last}\" -> \"{current}\" [ label=\"{n.Value}\" penwidth={p.ToString(CultureInfo.CurrentCulture).Replace(",", ".")} weight={w} ];");
          last = current;
        }
      }

      stb.AppendLine();

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
