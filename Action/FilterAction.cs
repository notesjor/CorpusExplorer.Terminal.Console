using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class FilterAction : AbstractAction
  {
    public override string Action => "query";
    public override string Description => "query - see help section [OUTPUT] for more information";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 2)
        return;

      Selection sub;
      if (args[0].StartsWith("FILE:"))
      {
        var lines = File.ReadAllLines(args[0].Replace("FILE:", string.Empty), Configuration.Encoding);
        var queries = lines.Select(QueryParser.Parse);
        sub = queries.Aggregate(selection,
          (current, q) => current.Create(new[] {q}, Path.GetFileNameWithoutExtension(args[1])));
      }
      else
      {
        var query = QueryParser.Parse(args[0]);
        if (query is FilterQueryUnsupportedParserFeature)
        {
          var s = args[0].Split(new[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
          if (s.Length != 2)
            return;

          UnsupportedParserFeatureHandler(selection, (FilterQueryUnsupportedParserFeature) query, args[1], writer);
          return;
        }

        sub = selection.Create(new[] {query}, Path.GetFileNameWithoutExtension(args[1]));
      }

      var export = new OutputAction();
      export.Execute(sub, new[] {args[1]}, writer);
    }

    private void UnsupportedParserFeatureAutosplit(Selection selection, FilterQueryUnsupportedParserFeature query,
      string output, AbstractTableWriter writer)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var outputOptions = output.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries);
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
        export.Execute(cluster,
          new[] {$"{form}#\"{Path.Combine(dir, $"{nam}_{cluster.Displayname.EnsureFileName()}{ext}")}\""}, writer);
    }

    private void UnsupportedParserFeatureHandler(Selection selection, FilterQueryUnsupportedParserFeature query,
      string output, AbstractTableWriter writer)
    {
      if (query.MetaLabel == "<:RANDOM:>")
        UnsupportedParserRandomFeature(selection, query, output, writer);
      else
        UnsupportedParserFeatureAutosplit(selection, query, output, writer);
    }

    private void UnsupportedParserRandomFeature(Selection selection, FilterQueryUnsupportedParserFeature query,
      string output, AbstractTableWriter writer)
    {
      var values = query.MetaValues?.ToArray();
      if (values?.Length != 1)
        return;

      var outputOptions = output.Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries);
      if (outputOptions.Length != 2)
        return;

      var block = selection.CreateBlock<RandomSelectionBlock>();
      block.DocumentCount = int.Parse(values[0].ToString());
      block.Calculate();

      var export = new OutputAction();
      export.Execute(block.RandomSelection, new[] {output}, writer);

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

      export.Execute(nega, new[] {$"{form}#\"{Path.Combine(dir, $"{nam}_inverse{ext}")}\""}, writer);
    }
  }
}