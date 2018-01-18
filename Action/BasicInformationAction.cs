using System.Collections.Generic;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class BasicInformationAction : AbstractAction
  {
    public override string Action => "basic-information";
    public override string Description => "basic-information - basic information tokens/sentences/documents";

    public override void Execute(Selection selection, string[] args)
    {
      WriteOutput("param\tvalue\r\n");
      WriteOutput($"tokens\t{selection.CountToken}\r\n");
      WriteOutput($"token-factor\t{1000000.0 / selection.CountToken}\r\n");
      WriteOutput($"sentences\t{selection.CountSentences}\r\n");
      WriteOutput($"documents\t{selection.CountDocuments}\r\n");
    }
  }
}