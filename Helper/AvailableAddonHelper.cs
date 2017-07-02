using System.Collections.Generic;
using System.Linq;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class AvailableAddonHelper
  {
    public static Dictionary<string, T> GetDictionary<T>(this IEnumerable<KeyValuePair<string, T>> dic)
      => dic.ToDictionary(x => x.Value.GetType().Name, x => x.Value);

    public static Dictionary<string, T> GetDictionary<T>(this IEnumerable<T> enumerable)
      => enumerable.ToDictionary(x => x.GetType().Name);
  }
}