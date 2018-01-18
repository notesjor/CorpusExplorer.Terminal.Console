using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaCategoriesAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> { "meta-categories" };

    public override void Execute(Selection selection, string[] args)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties();

      WriteOutput("meta-categories\r\n");
      foreach (var x in categories)
        WriteOutput($"{x}\r\n");
    }
  }
}