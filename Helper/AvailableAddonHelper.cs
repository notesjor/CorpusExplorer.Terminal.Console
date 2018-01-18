using System.Collections.Generic;
using System.Linq;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class AvailableAddonHelper
  {
    public static Dictionary<string, T> GetDictionary<T>(this IEnumerable<KeyValuePair<string, T>> dic)
    {
      var dictionary = new Dictionary<string, T>();
      foreach (var pair in dic)
        if (!dictionary.ContainsKey(pair.Value.GetType().Name))
          dictionary.Add(pair.Value.GetType().Name, pair.Value);
      return dictionary;
    }

    public static Dictionary<string, T> GetDictionary<T>(this IEnumerable<T> enumerable)
    {
      var dictionary = new Dictionary<string, T>();
      foreach (var unknown in enumerable)
        if (!dictionary.ContainsKey(unknown.GetType().Name))
          dictionary.Add(unknown.GetType().Name, unknown);
      return dictionary;
    }
  }
}