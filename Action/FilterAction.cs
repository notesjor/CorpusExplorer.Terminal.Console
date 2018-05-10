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

      var block = selection.CreateBlock<SelectionClusterBlock>();
      var split = values[0].ToString().Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length < 1 || split.Length > 3)
        return;

      switch (split[0])
      {
        case "TEXT":
        case "text":
        case "Text":
          block.ClusterGenerator = new SelectionClusterGeneratorStringValue();
          break;
        case "INT":
        case "int":
        case "Int":
          if (split.Length != 2)
            return;
          block.ClusterGenerator = new SelectionClusterGeneratorIntegerRange
          {
            Ranges = int.Parse(split[1]),
            AutoDetectMinMax = true
          };
          break;
        case "FLOAT":
        case "float":
        case "Float":
          block.ClusterGenerator = new SelectionClusterGeneratorDoubleRange
          {
            Ranges = int.Parse(split[1]),
            AutoDetectMinMax = true
          }; ;
          break;
        case "DATE":
        case "date":
        case "Date":
          switch (split[1])
          {
            case "CLUSTER":
              if (split.Length != 3)
                return;
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeRange
              {
                Ranges = int.Parse(split[2]),
                AutoDetectMinMax = true
              }; ;
              break;
            case "CEN":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeCenturyOnlyValue();
              break;
            case "DEC":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeDecateOnlyValue();
              break;
            case "Y":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeYearOnlyValue();
              break;
            case "YM":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeYearMonthOnlyValue();
              break;
            case "YMD":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeYearMonthDayOnlyValue();
              break;
            case "YMDH":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeYearMonthDayHourOnlyValue();
              break;
            case "YMDHM":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeYearMonthDayHourMinuteOnlyValue();
              break;
            case "ALL":
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeValue();
              break;
          }
          break;
      }
      block.MetadataKey = query.MetaLabel;
      block.Calculate();

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