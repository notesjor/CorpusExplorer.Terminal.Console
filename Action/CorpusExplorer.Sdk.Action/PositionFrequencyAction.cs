﻿using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class PositionFrequencyAction : IAction
  {
    public string Action => "position-frequency";

    public string Description => Resources.DescPositionFrequency;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var vm = new PositionFrequencyViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        LayerQueries = new[] { args[1] }
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}