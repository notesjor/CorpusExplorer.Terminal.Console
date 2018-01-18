using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class DocumentCountAction : AbstractAction
  {
    public override string Action => "how-many-documents";
    public override string Description => "how-many-documents - sum of all documents";

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput(selection.CountDocuments.ToString());
    }
  }
}