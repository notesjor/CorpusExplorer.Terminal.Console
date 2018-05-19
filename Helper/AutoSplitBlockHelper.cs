﻿using System;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Generator;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class AutoSplitBlockHelper
  {
    public static SelectionClusterBlock RunAutoSplit(Selection selection, FilterQueryUnsupportedParserFeature query,
      object[] values)
    {
      var block = selection.CreateBlock<SelectionClusterBlock>();
      var split = values[0].ToString().Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length < 1 || split.Length > 3)
        return null;

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
            return null;
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
          };
          ;
          break;
        case "DATE":
        case "date":
        case "Date":
          switch (split[1])
          {
            case "CLUSTER":
              if (split.Length != 3)
                return null;
              block.ClusterGenerator = new SelectionClusterGeneratorDateTimeRange
              {
                Ranges = int.Parse(split[2]),
                AutoDetectMinMax = true
              };
              ;
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
      return block;
    }
  }
}