using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KwitAction : IAction
  {
    public string Action => "kwit";

    public string Description => Resources.DescKwit;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 4)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);

      var query = GetQuery(args[0], ref queries);

      var vm = new TextFlowSearchViewModel
      {
        Selection = selection,
        LayerDisplayname = args[1],
        MinFrequency = int.Parse(args[2]),
        LayerQuery = query,
        AutoJoin = true,
        HighlightCooccurrences = false
      };
      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(vm.DiscoveredConnections.ToArray()));
    }

    private AbstractFilterQuery GetQuery(string layerDisplayname, ref List<string> queries)
    {
      var filter = new HashSet<string>{ "ANY", "FIRST", "SENTENCE", "PHRASE" };
      if (!filter.Contains(queries[0]))
        return new FilterQuerySingleLayerExactPhrase
        {
          LayerDisplayname = layerDisplayname,
          LayerQueries = queries
        };

      var match = queries[0];
      queries.RemoveAt(0);

      switch (match)
      {
        case "ANY":
          return new FilterQuerySingleLayerAnyMatch
          {
            LayerDisplayname = layerDisplayname,
            LayerQueries = queries
          };
        case "FIRST":
          return new FilterQuerySingleLayerFirstAndAnyOtherMatch
          {
            LayerDisplayname = layerDisplayname,
            LayerQueries = queries
          };
        case "SENTENCE":
          return new FilterQuerySingleLayerAllInOneSentence
          {
            LayerDisplayname = layerDisplayname,
            LayerQueries = queries
          };
        default:
          return new FilterQuerySingleLayerExactPhrase
          {
            LayerDisplayname = layerDisplayname,
            LayerQueries = queries
          };
      }
    }
  }
}