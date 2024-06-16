using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;
using System.Linq;

namespace CorpusExplorer.Sdk.Action
{
  public class PositionFrequencyTransitionAction : IAction
  {
    public string Action => "position-frequency-transition";

    public string Description => "position-frequency-transition [LAYER1] [WORDS] - generates a transition-tree (DOT-Format) for [WORDS]-Phrase";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var query = args.ToList();
      query.RemoveAt(0);

      var block = selection.CreateBlock<PositionFrequencyTransitionBlock>();
      block.LayerDisplayname = args[0];
      block.LayerQueries = query;
      block.Calculate();

      writer.WriteDirectThroughStream(ConvertToDigraphHelper.Convert(block.Result));
    }
  }
}