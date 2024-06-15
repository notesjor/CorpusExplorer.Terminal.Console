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

      var vm = new TextFlowSearchViewModel
      {
        Selection = selection,
        Layer1Displayname = args[0],
        Layer2Displayname = args[1],
        MinFrequency = int.Parse(args[2]),
        LayerQueryPhrase = queries,
        AutoJoin = true,
        HighlightCooccurrences = false
      };
      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(vm.DiscoveredConnections.ToArray()));
    }
  }
}