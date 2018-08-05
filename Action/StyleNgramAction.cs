using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class StyleNgramAction : AbstractAction
  {
    public override string Action => "style-ngram";

    public override string Description =>
      "style-ngram [LAYER] [META] [N] [minFREQ] - style analytics based on ngram";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 4)
        return;

      var vm = new ClusterMetadataByNGramViewModel
      {
        Selection = selection,
        LayerDisplayname = args[0],
        MetadataKey = args[1],
        NGramSize = int.Parse(args[2]),
        NGramMinFrequency = int.Parse(args[3])
      };
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetCrossDataTable());
    }
  }
}