using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class DocumentCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"how-many-documents"};

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput(selection.CountDocuments.ToString());
    }
  }
}