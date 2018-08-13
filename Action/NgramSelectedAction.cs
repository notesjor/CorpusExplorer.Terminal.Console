using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class NGramSelectedAction : IAddonConsoleAction
  {
    public string Action => "n-gram-select";

    public string Description =>
      "n-gram-select [N] [LAYER] [WORDS] - all [N]-grams on [LAYER] containing [WORDS].";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 3)
        return;

      var vm = new Ngram1LayerSelectiveViewModel
      {
        Selection = selection,
        NGramSize = int.Parse(args[0]),
        LayerDisplayname = args[1]
      };

      var queries = args.ToList();
      queries.RemoveAt(0);
      queries.RemoveAt(0);
      vm.LayerQueries = queries;

      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}