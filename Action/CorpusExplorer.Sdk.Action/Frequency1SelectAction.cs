using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class Frequency1SelectAction : IAction
  {
    public string Action => "frequency1-select";

    public string Description => Resources.DescFrequency1Selected;

    internal bool Normalize { get; set; } = true;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var vm = new Frequency1LayerViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0]
      };
      vm.Execute();

      var lst = new List<string>(args);
      lst.RemoveAt(0);
      var hsh = new HashSet<string>(lst);

      var div = Normalize ? selection.CountToken / 1000000d : 0d;

      switch (hsh.Count)
      {
        case 1 when hsh.ToArray()[0].StartsWith("FILE:"):
          ExecuteFileQuery(selection.Displayname, writer, vm, div, hsh.ToArray()[0].Replace("FILE:", ""));
          break;
        case 1 when hsh.ToArray()[0].StartsWith("G-FILE:"):
          ExecuteGroupFileQuery(selection.Displayname, writer, vm, div, hsh.ToArray()[0].Replace("G-FILE:", ""));
          break;
        case 1 when hsh.ToArray()[0].StartsWith("SDM:"):
          ExecuteSdmFileQuery(selection.Displayname, writer, vm, div, hsh.ToArray()[0].Replace("SDM:", ""));
          break;
        default:
          {
            if (hsh.Count >= 1)
              ExecuteSimpleQuery(selection.Displayname, writer, vm, div, hsh);
            break;
          }
      }
    }

    private void ExecuteFileQuery(string tid, AbstractTableWriter writer, Frequency1LayerViewModel vm, double div,
                                  string path)
    {
      var lines = File.ReadAllLines(path, Configuration.Encoding);

      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add(Resources.Frequency, typeof(double));
      if(div > 0)
        res.Columns.Add(Resources.FrequencyRel, typeof(double));

      res.BeginLoadData();

      foreach (var line in lines)
      {
        if (!vm.Frequency.ContainsKey(line))
          continue;

        var sum = vm.Frequency[line];
        if (div > 0)
          res.Rows.Add(line, sum, sum / div);
        else
          res.Rows.Add(line, sum, sum);
      }

      res.EndLoadData();

      writer.WriteTable(tid, res);
    }

    private void ExecuteGroupFileQuery(string tid, AbstractTableWriter writer, Frequency1LayerViewModel vm, double div,
                                  string path)
    {
      var lines = File.ReadAllLines(path, Configuration.Encoding);

      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add(Resources.Frequency, typeof(double));
      if (div > 0)
        res.Columns.Add(Resources.FrequencyRel, typeof(double));

      res.BeginLoadData();

      foreach (var line in lines)
      {
        var split = line.Split(Splitter.Tab, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length < 2)
          continue;

        var grp = split[0];
        var sum = 0d;
        for (var i = 1; i < split.Length; i++)
          if (vm.Frequency.ContainsKey(split[i]))
            sum += vm.Frequency[split[i]];

        if (div > 0)
          res.Rows.Add(line, sum, sum / div);
        else
          res.Rows.Add(line, sum, sum);
      }

      res.EndLoadData();

      writer.WriteTable(tid, res);
    }

    private void ExecuteSdmFileQuery(string tid, AbstractTableWriter writer, Frequency1LayerViewModel vm, double div,
                                     string path)
    {
      var lines = File.ReadAllLines(path, Configuration.Encoding);

      var res = new DataTable();

      res.Columns.Add(Resources.Category, typeof(string));
      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add(Resources.Frequency, typeof(double));
      if(div > 0)
        res.Columns.Add(Resources.FrequencyRel, typeof(double));
      res.Columns.Add(Resources.Value, typeof(double));

      res.BeginLoadData();
      var match = 0d;

      foreach (var line in lines)
      {
        var split = line.Split(Splitter.Tab, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length != 3)
          continue;

        var word = split[1];
        if (word.StartsWith("*")) // Postfix
        {
          foreach (var w in GetSdmPostfixQuery(word.Substring(1), vm.Frequency))
          {
            if (div > 0)
              res.Rows.Add(split[0], w.Key, w.Value, w.Value / div, split[2]);
            else
              res.Rows.Add(split[0], w.Key, w.Value, split[2]);
            match += w.Value;
          }
        }
        else if (word.EndsWith("*")) // Prefix
        {
          foreach (var w in GetSdmPrefixQuery(word.Substring(0, word.Length - 1), vm.Frequency))
          {
            if (div > 0)
              res.Rows.Add(split[0], w.Key, w.Value, w.Value / div, split[2]);
            else
              res.Rows.Add(split[0], w.Key, w.Value, split[2]);
            match += w.Value;
          }
        }
        else if (vm.Frequency.ContainsKey(word))
        {
          if (div > 0)
            res.Rows.Add(split[0], word, vm.Frequency[word], vm.Frequency[word] / div, split[2]);
          else
            res.Rows.Add(split[0], word, vm.Frequency[word], split[2]);
          match += vm.Frequency[word];
        }
      }

      var other = vm.Selection.CountToken - match;
      if (div > 0)
        res.Rows.Add(Resources.Other, Resources.TokenCount, other, other / div, 0);
      else
        res.Rows.Add(Resources.Other, Resources.TokenCount, other, 0);

      res.EndLoadData();

      writer.WriteTable(tid, res);
    }

    private Dictionary<string, double> GetSdmPostfixQuery(string query, Dictionary<string, double> vm)
    {
      return vm.Where(x => x.Key.EndsWith(query)).ToDictionary(x => x.Key, x => x.Value);
    }

    private Dictionary<string, double> GetSdmPrefixQuery(string query, Dictionary<string, double> vm)
    {
      return vm.Where(x => x.Key.StartsWith(query)).ToDictionary(x => x.Key, x => x.Value);
    }

    private static void ExecuteSimpleQuery(string tid, AbstractTableWriter writer, Frequency1LayerViewModel vm,
                                           double div, HashSet<string> hsh)
    {
      var res = new DataTable();

      res.Columns.Add(vm.LayerDisplayname, typeof(string));
      res.Columns.Add(Resources.Frequency, typeof(double));
      if(div > 0)
        res.Columns.Add(Resources.FrequencyRel, typeof(double));

      res.BeginLoadData();

      foreach (var f in vm.Frequency.Where(f => hsh.Contains(f.Key)))
        if(div > 0)
          res.Rows.Add(f.Key, f.Value, f.Value / div);
        else
          res.Rows.Add(f.Key, f.Value);

      res.EndLoadData();

      writer.WriteTable(tid, res);
    }
  }
}