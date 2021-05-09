using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Xml.Extensions
{
  public static class SourcesExtension
  {
    public static IEnumerable<annotate> annotate(this sources obj)
      => obj.Items.OfType<annotate>();

    public static IEnumerable<import> import(this sources obj)
      => obj.Items.OfType<import>();
  }
}