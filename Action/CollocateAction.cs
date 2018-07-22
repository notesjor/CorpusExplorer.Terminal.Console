﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class CollocateAction: AbstractAction
  {
    public override string Action => "position-frequency";

    public override string Description =>
      "position-frequency [LAYER1] [WORD] - left/right position of words around [WORD]";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var vm = new PositionFrequencyViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0]
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
