using System.Collections.Generic;
using System.Linq;

namespace CorpusExplorer.Sdk.Action.Helper
{
  public class KwicSpanHelper
  {
    public int SentencePre { get; }
    public int SentencePost { get; }
    public string[] CleanArguments {get; }

    public KwicSpanHelper(IEnumerable<string> args)
    {
      SentencePre = SentencePost = 0;

      var list = args.ToList();
      var clean = new List<int>();

      for (var i = 0; i < list.Count; i++)
      {
        var item = list[i];
        if (item.StartsWith("SPAN-") && int.TryParse(item.Replace("SPAN-", ""), out var pre) && pre >= 0)
        {
          SentencePre = pre;
          clean.Add(i);
        }
        else if (item.StartsWith("SPAN+") && int.TryParse(item.Replace("SPAN+", ""), out var post) && post >= 0)
        {
          SentencePost = post;
          clean.Add(i);
        }
      }

      for (var i = clean.Count - 1; i > -1; i--)
        list.RemoveAt(i);

      CleanArguments = list.ToArray();
    }
  }
}
