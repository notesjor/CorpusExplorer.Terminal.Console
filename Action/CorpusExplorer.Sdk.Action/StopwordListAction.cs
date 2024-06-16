using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class StopwordListAction : IAction
  {
    public string Action => "stopword";

    public string Description => "stopword {TRUE/FALSE} {LAYER1} {LAYER2} {WORDS} - generate a stopword-list for {LAYER2} based on {LAYER1}/{WORDS} - no {LAYER1/2}+{WORDS} = default / [TRUE] => lower case";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null)
        return;

      bool lowerCase;
      string layer1, layer2;
      string[] queries;

      if (args.Length == 0)
      {
        lowerCase = true;
        layer1 = "POS";
        layer2 = "Wort";
        queries = new[] { "$,", "$.", "$(", "ART", "KON", "KOUS", "KOKOM", "PDAT", "PRF", "PPER", "PPOSAT", "PPOSS", "PTKZU", "PWAV", "PWS", "PTKA", "PDS", "APZR", "PTKANT", "XY", "ITJ", "PRELAT", "PRELS", "APPRART" };
      }
      else
      {
        if (args.Length < 4)
          return;

        lowerCase = bool.Parse(args[0]);
        layer1 = args[1];
        layer2 = args[2];
        queries = args.Skip(3).ToArray();
      }

      var block = selection.CreateBlock<CorrespondingLayerValueBlock>();
      block.Layer1Displayname = layer1;
      block.Layer2Displayname = layer2;
      block.Calculate();

      var res = new HashSet<string>();
      foreach (var x in queries)
      {
        if (!block.CorrespondingLayerValues.ContainsKey(x))
          continue;

        var values = block.CorrespondingLayerValues[x];

        if (lowerCase)
          foreach (var v in values)
            res.Add(v.Trim().ToLower());
        else
          foreach (var v in values)
            res.Add(v.Trim());
      }

      var dt = new DataTable();
      dt.Columns.Add("Stopword", typeof(string));

      dt.BeginLoadData();
      foreach (var x in res)
        dt.Rows.Add(x);
      dt.EndLoadData();

      writer.WriteTable(dt);
    }
  }
}
