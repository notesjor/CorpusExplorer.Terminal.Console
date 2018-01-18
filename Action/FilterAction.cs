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

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : AbstractAction
  {
    public override string Action => "query";
    public override string Description => "query - see help section [OUTPUT] for more information";

    public override void Execute(Selection selection, string[] args)
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
        var lines = File.ReadAllLines(s[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        sub = queries.Aggregate(selection, (current, q) => current.Create(new[] { q }, Path.GetFileNameWithoutExtension(s[1])));
      }
      else
      {
        var query = QueryParser.Parse(s[0]);
        if (query is FilterQueryUnsupportedParserFeature)
        {
          UnsupportedParserFeatureHandler(selection, (FilterQueryUnsupportedParserFeature)query, s[1]);
          return;
        }

        sub = selection.Create(new[] { query }, Path.GetFileNameWithoutExtension(s[1]));
      }

      var export = new OutputAction();
      export.Execute(sub, new[] { s[1] });
    }

    private void UnsupportedParserFeatureHandler(Selection selection, FilterQueryUnsupportedParserFeature query, string path)
    {
      if (query.MetaLabel == "<:RANDOM:>")
        UnsupportedParserRandomFeature(selection, query, path);
      else
        UnsupportedParserFeatureAutosplit(selection, query, path);
    }

    private void UnsupportedParserRandomFeature(Selection selection, FilterQueryUnsupportedParserFeature query, string path)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var block = selection.CreateBlock<RandomSelectionBlock>();
      block.DocumentCount = int.Parse(values[0].ToString());
      block.Calculate();

      var export = new OutputAction();
      export.Execute(block.RandomSelection, new[] { path });

      var dir = Path.GetDirectoryName(path);
      var nam = Path.GetFileNameWithoutExtension(path);
      var ext = Path.GetExtension(path);

      var none = new HashSet<Guid>(block.RandomSelection.DocumentGuids);
      var list = selection.DocumentGuids.Where(dsel => !none.Contains(dsel));
      var nega = selection.CreateTemporary(list);

      export.Execute(nega, new[] { Path.Combine(dir, $"{nam}_inverse{ext}") });
    }

    private void UnsupportedParserFeatureAutosplit(Selection selection, FilterQueryUnsupportedParserFeature query, string path)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var block = selection.CreateBlock<SelectionClusterBlock>();
      var split = values[0].ToString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length < 1 || split.Length > 3)
        return;

      switch (split[0])
      {
        case "TEXT":
        case "text":
        case "Text":
          block.ClusterGenerator = new SelectionClusterGeneratorByStringValue();
          break;
        case "INT":
        case "int":
        case "Int":
          if (split.Length != 2)
            return;
          block.ClusterGenerator = new SelectionClusterGeneratorByIntegerRange().Configure(int.Parse(split[1]));
          break;
        case "FLOAT":
        case "float":
        case "Float":
          block.ClusterGenerator = new SelectionClusterGeneratorByDoubleRange().Configure(int.Parse(split[1]));
          break;
        case "DATE":
        case "date":
        case "Date":
          switch (split[1])
          {
            case "CLUSTER":
              if(split.Length != 3)
                return;
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeRange().Configure(int.Parse(split[2]));
              break;
            case "Y":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeYearOnlyValue();
              break;
            case "YM":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeYearMonthOnlyValue();
              break;
            case "YMD":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeYearMonthDayOnlyValue();
              break;
            case "YMDH":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeYearMonthDayHourOnlyValue();
              break;
            case "YMDHM":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeYearMonthDayHourMinuteOnlyValue();
              break;
            case "ALL":
              block.ClusterGenerator = new SelectionClusterGeneratorByDateTimeValue();
              break;
          }
          break;
      }
      block.MetadataKey = query.MetaLabel;
      block.Calculate();

      var dir = Path.GetDirectoryName(path);
      var nam = Path.GetFileNameWithoutExtension(path);
      var ext = Path.GetExtension(path);

      var export = new OutputAction();
      foreach (var cluster in block.GetSelectionClusters())
      {
        export.Execute(cluster, new[] { Path.Combine(dir, $"{nam}_{cluster.Displayname.EnsureFileName()}{ext}") });
      }
    }
  }
}