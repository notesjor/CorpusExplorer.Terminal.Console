using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Xml.Extensions
{
  public static class QueriesExtension
  {
    public static IEnumerable<query> query(this queries obj)
    {
      return obj.Items.OfType<query>();
    }

    public static IEnumerable<queryGroup> queryGroup(this queries obj)
    {
      return obj.Items.OfType<queryGroup>();
    }

    public static IEnumerable<queryBuilder> queryBuilder(this queries obj)
    {
      return obj.Items.OfType<queryBuilder>();
    }
  }
}