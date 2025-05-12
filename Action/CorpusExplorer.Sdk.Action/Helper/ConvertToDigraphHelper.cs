using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CorpusExplorer.Sdk.Action.Helper
{
  public static class ConvertToDigraphHelper
  {
    public static string Convert(Dictionary<string, Dictionary<string, double>> connections)
     => Convert((from connection in connections from connection2 in connection.Value select new Tuple<string, int, string>(connection.Key, (int)connection2.Value, connection2.Key)));

    public static string Convert(IEnumerable<Tuple<string, int, string>> connections)
    {
      if (connections == null || !connections.Any())
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

    private static string Filter(object content)
    {
      return content.ToString().Replace("\"", "''");
    }
  }
}
