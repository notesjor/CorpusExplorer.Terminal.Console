using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class MetaCategoriesAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"meta-categories", "metacategories"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties();

      WriteOutput("meta-categories\r\n");
      foreach (var x in categories)
        WriteOutput($"{x}\r\n");
    }
  }
}