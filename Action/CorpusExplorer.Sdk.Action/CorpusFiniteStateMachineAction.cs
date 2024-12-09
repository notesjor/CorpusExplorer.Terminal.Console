using System.Text;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class CorpusFiniteStateMachineAction : IAction
  {
    public string Action => "corpus-fsm";
    public string Description => "corpus-fsm [ORDER] [ENTITY] [STATE] - generates a fine-state-maschine based on corpus meta-data.";
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 3)
        return;

      var vm = new CorpusFiniteStateMachineViewModel
      {
        Selection = selection,
        MetadataKeyTimestamp = args[0],
        MetadataKeyEntity = args[1],
        MetadataKeyLevel = args[2]
      };
      vm.Execute();

      var stb = new StringBuilder();
      stb.AppendLine("digraph G {");
      foreach (var edge in vm.ConnectionsAggregated)
        stb.AppendLine($"  \"{edge.Key}\" -> \"{edge.Value}\"");
      stb.AppendLine("}");

      writer.WriteDirectThroughStream(stb.ToString());
    }
  }
}