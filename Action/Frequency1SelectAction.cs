using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class Frequency1SelectAction : AbstractAction
  {
    public override string Action => "frequency1-select";
    public override string Description => "frequency1-select [LAYER1] [WORDS] - count token frequency on 1 [LAYER] - [WORDS] = space separated tokens";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new Frequency1LayerViewModel { Selection = selection };
      if (args != null && args.Length == 1)
        vm.LayerDisplayname = args[0];
      vm.Analyse();

      var lst = new List<string>(args);
      lst.RemoveAt(0);
      var hsh = new HashSet<string>(lst);

      var div = vm.Frequency.Select(x => x.Value).Sum() / 1000000d;
      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add("Frequenz", typeof(double));
      res.Columns.Add("Frequenz (relativ)", typeof(double));

      res.BeginLoadData();

      foreach (var f in vm.Frequency)
        if (hsh.Contains(f.Key))
          res.Rows.Add(f.Key, f.Value, f.Value / div);

      res.EndLoadData();

      writer.WriteTable(res);
    }
  }
}