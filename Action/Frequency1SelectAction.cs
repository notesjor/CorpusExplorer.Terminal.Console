using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Ecosystem.Model;
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

      switch (hsh.Count)
      {
        case 1 when hsh.ToArray()[0].StartsWith("FILE:"):
          ExecuteFileQuery(writer, vm, div, hsh.ToArray()[0].Replace("FILE:", ""));
          break;
        case 1 when hsh.ToArray()[0].StartsWith("SDM:"):
          ExecuteSdmFileQuery(writer, vm, div, hsh.ToArray()[0].Replace("SDM:", ""));
          break;
        default:
          ExecuteSimpleQuery(writer, vm, div, hsh);
          break;
      }
    }

    private void ExecuteFileQuery(AbstractTableWriter writer, Frequency1LayerViewModel vm, double div, string path)
    {
      var lines = File.ReadAllLines(path, Configuration.Encoding);

      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add("Frequenz", typeof(double));
      res.Columns.Add("Frequenz (relativ)", typeof(double));

      res.BeginLoadData();

      foreach (var line in lines)
      {
        var split = line.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length < 2)
          continue;

        var grp = split[0];
        var sum = 0d;
        for (var i = 1; i < split.Length; i++)
          if (vm.Frequency.ContainsKey(split[i]))
            sum += vm.Frequency[split[i]];

        res.Rows.Add(grp, sum, sum / div);
      }

      res.EndLoadData();

      writer.WriteTable(res);
    }

    private void ExecuteSdmFileQuery(AbstractTableWriter writer, Frequency1LayerViewModel vm, double div, string path)
    {
      var lines = File.ReadAllLines(path, Configuration.Encoding);

      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add("Frequenz", typeof(double));
      res.Columns.Add("Frequenz (relativ)", typeof(double));
      res.Columns.Add("Wert", typeof(double));

      res.BeginLoadData();

      foreach (var line in lines)
      {
        var split = line.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 3)
          continue;

        if (vm.Frequency.ContainsKey(split[1]))
          res.Rows.Add(split[0], vm.Frequency[split[1]], vm.Frequency[split[1]] / div, split[2]);
      }

      res.EndLoadData();

      writer.WriteTable(res);
    }

    private static void ExecuteSimpleQuery(AbstractTableWriter writer, Frequency1LayerViewModel vm, double div, HashSet<string> hsh)
    {
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