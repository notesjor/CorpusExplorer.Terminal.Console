using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaDocumentAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> { "meta-by-document" };

    public override void Execute(Selection selection, string[] args)
    {
      var categories = selection.GetDocumentMetadataPrototypeOnlyProperties().ToArray();
      WriteOutput($"GUID\t{string.Join("\t", categories)}\r\n");
      foreach (var dsel in selection.DocumentGuids)
      {
        var meta = selection.GetDocumentMetadata(dsel);
        var output = new StringBuilder($"{dsel:N}\t");

        for (var i = 0; i < categories.Length; i++)
        {
          if (meta.ContainsKey(categories[i]) && meta[categories[i]] != null)
            output.Append($"{meta[categories[i]]}{(i + 1 == categories.Length ? "\r\n" : "\t")}");
          else
            output.Append($"{(i + 1 == categories.Length ? "\r\n" : "\t")}");
        }

        WriteOutput(output.ToString());
      }
    }
  }
}