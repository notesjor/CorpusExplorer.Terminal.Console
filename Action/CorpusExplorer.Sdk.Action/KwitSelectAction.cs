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
  public class KwitSelectAction : IAction
  {
    public string Action => "kwit-n";

    public string Description => Resources.DescKwitSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 6)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      queries.RemoveAt(0);

      var vm = new TextFlowSearchWithRangeSelectionViewModel
      {
        Selection = selection,
        Layer1Displayname = args[0],
        Layer2Displayname = args[1],
        MinFrequency = int.Parse(args[2]),
        Pre = int.Parse(args[3]),
        Post = int.Parse(args[4]),
        LayerQueryPhrase = queries,
        AutoJoin = true,
        HighlightCooccurrences = false,        
      };
      vm.Execute();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(vm.DiscoveredConnections.ToArray()));
    }    
  }
}