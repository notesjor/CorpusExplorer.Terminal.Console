using System.Collections.Generic;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class DocumentCountAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"many-document"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      WriteOutput(selection.CountDocuments.ToString());
    }
  }
}