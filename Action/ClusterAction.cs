﻿using System;
using System.Collections.Generic;
using System.Data;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class ClusterAction : IAddonConsoleAction
  {
    internal Dictionary<string, IAddonConsoleAction> _actions;
    public string Action => "cluster";
    public string Description => "cluster [QUERY] [TASK] [ARGUMENTS] - executes a [TASK] for every cluster (generated by [QUERY])";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 2 || args[1].ToLower() == "cluster")
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;      

      var selections = UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature) query);
      if (selections == null)
        return;

      var nargs = new List<string>(args);
      nargs.RemoveAt(0);
      var task = nargs[0];
      if (!_actions.ContainsKey(task))
        return;

      nargs.RemoveAt(0);
      foreach (var s in selections)
      {
        _actions[task].Execute(s, nargs.ToArray(), writer);
      }
    }
  }
}