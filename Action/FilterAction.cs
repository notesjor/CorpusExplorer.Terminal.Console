using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Generator;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Generator.Abstract;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : AbstractAction
  {
    public override string Action => "query";
    public override string Description => "query - see help section [OUTPUT] for more information";

    public override void Execute(Selection selection, string[] args)
    {
      var a = args?.ToArray();
      if (a == null || a.Length != 2)
        return;

      Selection sub;
      if (a[0].StartsWith("FILE:"))
      {
        var lines = File.ReadAllLines(a[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        sub = queries.Aggregate(selection, (current, q) => current.Create(new[] { q }, Path.GetFileNameWithoutExtension(a[1])));
      }
      else
      {
        var query = QueryParser.Parse(a[0]);
        if (query is FilterQueryUnsupportedParserFeature)
        {
          var s = a[0].Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
          if (s.Length != 2)
            return;

          UnsupportedParserFeatureHandler(selection, (FilterQueryUnsupportedParserFeature)query, a[1]);
          return;
        }

        sub = selection.Create(new[] { query }, Path.GetFileNameWithoutExtension(a[1]));
      }

      var export = new OutputAction();
      export.Execute(sub, new[] { a[1] });
    }

    private void UnsupportedParserFeatureHandler(Selection selection, FilterQueryUnsupportedParserFeature query, string output)
    {
      if (query.MetaLabel == "<:RANDOM:>")
        UnsupportedParserRandomFeature(selection, query, output);
      else
        UnsupportedParserFeatureAutosplit(selection, query, output);
    }

    private void UnsupportedParserRandomFeature(Selection selection, FilterQueryUnsupportedParserFeature query, string output)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var outputOptions = output.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
      if (outputOptions.Length != 2)
        return;

      var block = selection.CreateBlock<RandomSelectionBlock>();
      block.DocumentCount = int.Parse(values[0].ToString());
      block.Calculate();

      var export = new OutputAction();
      export.Execute(block.RandomSelection, new[] { output });

      var form = outputOptions[0];
      var path = outputOptions[1];
      var nam = Path.GetFileNameWithoutExtension(path);
      var ext = Path.GetExtension(path);
      var dir = Path.GetDirectoryName(path);
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      var none = new HashSet<Guid>(block.RandomSelection.DocumentGuids);
      var list = selection.DocumentGuids.Where(dsel => !none.Contains(dsel));
      var nega = selection.CreateTemporary(list);

      export.Execute(nega, new[] { $"{form}#\"{Path.Combine(dir, $"{nam}_inverse{ext}")}\"" });
    }

    private void UnsupportedParserFeatureAutosplit(Selection selection, FilterQueryUnsupportedParserFeature query, string output)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var outputOptions = output.Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
      if (outputOptions.Length != 2)
        return;

      var block = AutoSplitBlockHelper.RunAutoSplit(selection, query, values);

      var form = outputOptions[0];
      var path = outputOptions[1];
      var dir = Path.GetDirectoryName(path);
      var nam = Path.GetFileNameWithoutExtension(path);
      var ext = Path.GetExtension(path);

      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      var export = new OutputAction();
      foreach (var cluster in block.GetSelectionClusters())
      {
        export.Execute(cluster, new[] { $"{form}#\"{Path.Combine(dir, $"{nam}_{cluster.Displayname.EnsureFileName()}{ext}")}\"" });
      }
    }
  }
}