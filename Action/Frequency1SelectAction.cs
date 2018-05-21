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
      if (args == null || args.Length < 2)
        return;

      vm.LayerDisplayname = args[0];
      vm.Analyse();

      var lst = new List<string>(args);
      lst.RemoveAt(0);
      var hsh = new HashSet<string>(lst);

      var div = vm.Frequency.Select(x => x.Value).Sum() / 1000000d;

      if (hsh.Count == 1 && hsh.ToArray()[0].StartsWith("FILE:"))
        ExecuteFileQuery(writer, vm, div, hsh.ToArray()[0].Replace("FILE:", ""));
      else if (hsh.Count == 1 && hsh.ToArray()[0].StartsWith("SDM:"))
        ExecuteSdmFileQuery(writer, vm, div, hsh.ToArray()[0].Replace("SDM:", ""));
      else
        ExecuteSimpleQuery(writer, vm, div, hsh);
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

      res.Columns.Add("Kategorie", typeof(string));
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

        var word = split[1];
        if (word.StartsWith("*")) // Postfix
          foreach (var w in GetSdmPostfixQuery(word.Substring(1), vm.Frequency))
            res.Rows.Add(split[0], w.Key, w.Value, w.Value / div, split[2]);
        else if (word.EndsWith("*")) // Prefix
          foreach (var w in GetSdmPrefixQuery(word.Substring(0, word.Length - 1), vm.Frequency))
            res.Rows.Add(split[0], w.Key, w.Value, w.Value / div, split[2]);
        else if (vm.Frequency.ContainsKey(word))
          res.Rows.Add(split[0], word, vm.Frequency[word], vm.Frequency[word] / div, split[2]);
      }

      res.EndLoadData();

      writer.WriteTable(res);
    }

    private Dictionary<string, double> GetSdmPostfixQuery(string query, Dictionary<string, double> vm) 
      => vm.Where(x => x.Key.EndsWith(query)).ToDictionary(x => x.Key, x => x.Value);

    private Dictionary<string, double> GetSdmPrefixQuery(string query, Dictionary<string, double> vm) 
      => vm.Where(x => x.Key.StartsWith(query)).ToDictionary(x => x.Key, x => x.Value);

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