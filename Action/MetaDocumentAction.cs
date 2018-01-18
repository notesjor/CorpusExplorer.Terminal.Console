using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class MetaDocumentAction : AbstractAction
  {
    public override string Action => "meta-by-document";
    public override string Description => "meta-by-document - list all documents with meta-data";

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