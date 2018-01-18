using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaCategoriesAction : AbstractAction
  {
    public override string Action => "meta-categories";
    public override string Description => "meta-categories - all available names for meta categories";

    public override void Execute(Selection selection, string[] args)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties();

      WriteOutput("meta-categories\r\n");
      foreach (var x in categories)
        WriteOutput($"{x}\r\n");
    }
  }
}