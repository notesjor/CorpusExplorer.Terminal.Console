using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Xml.Extensions
{
  public static class MetaExtension
  {
    public static IEnumerable<meta> meta(this head obj) 
      => obj?.Items?.OfType<meta>() ?? new meta[0];

    public static IEnumerable<config> config(this head obj)
      => obj?.Items?.OfType<config>() ?? new config[0];

    public static IEnumerable<meta> meta(this object[] obj)
      => obj?.OfType<meta>() ?? new meta[0];

    public static IEnumerable<config> config(this object[] obj)
      => obj?.OfType<config>() ?? new config[0];
  }
}
