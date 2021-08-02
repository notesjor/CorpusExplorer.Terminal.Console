﻿using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicNamedEntityAction : IAction
  {
    public string Action => "kwic-ner";
    public string Description => Resources.DescKwicNer;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 1)
        return;

      var vm = new NamedEntityDetectionViewModel
      {
        Selection = selection,
        Model = Blocks.NamedEntityRecognition.NamedEntityRecognitionModel.Load(args[0])
      };

      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetKwicDataTable());
    }
  }
}