using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bcs.IO;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "query", "filter", "select", "snapshot" };

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var a = args?.ToArray();
      if (a == null || a.Length != 1)
        return;

      var s = a[0].Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
      if (s.Length != 2)
        return;

      Selection sub;
      if (s[0].StartsWith("FILE:"))
      {
        var lines = FileIO.ReadLines(s[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        sub = queries.Aggregate(selection, (current, q) => current.Create(new[] {q}, Path.GetFileNameWithoutExtension(s[1])));
      }
      else
      {
        var query = QueryParser.Parse(s[0]);
        sub = selection.Create(new[] { query }, Path.GetFileNameWithoutExtension(s[1]));
      }

      var export = new OutputAction();
      export.Execute(sub, new[] { s[1] });
    }
  }
}