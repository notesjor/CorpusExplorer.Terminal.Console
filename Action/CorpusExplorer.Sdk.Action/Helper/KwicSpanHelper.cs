using System.Collections.Generic;
using System.Linq;

namespace CorpusExplorer.Sdk.Action.Helper
{
  public class KwicSpanHelper
  {
    public bool EnableMeta { get; } = false;
    public int SentencePre { get; }
    public int SentencePost { get; }
    public string[] CleanArguments { get; }

    public KwicSpanHelper(IEnumerable<string> args)
    {
      SentencePre = SentencePost = 0;

      var input = args.ToList();
      var output = new List<string>();

      foreach (var item in input)
      {
        if (item == "META")
          EnableMeta = true;
        else if (item.StartsWith("SPAN-") && int.TryParse(item.Replace("SPAN-", ""), out var pre) && pre >= 0)
          SentencePre = pre;
        else if (item.StartsWith("SPAN+") && int.TryParse(item.Replace("SPAN+", ""), out var post) && post >= 0)
          SentencePost = post;
        else
          output.Add(item);
      }

      CleanArguments = output.ToArray();
    }
  }
}
