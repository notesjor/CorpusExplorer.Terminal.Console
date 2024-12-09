using CorpusExplorer.Sdk.Addon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class NGramCharAction : IAction
  {
    public string Action => "ngram-char";
    public string Description => "ngram-char [N] {LAYER} - [N] sized Char-N-gram based on {LAYER} (default: Wort)";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 1)
        return;

      var vm = new NgramPhoneticViewModel
      {
        Selection = selection,
        NGramSize = int.Parse(args[0]),
        LayerDisplayname = args.Length > 2 ? args[1] : "Wort"
      };
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
