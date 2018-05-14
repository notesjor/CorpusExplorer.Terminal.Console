using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class XmlValueHelper
  {
    public static string CleanXmlValue(this string[] value) => CleanXmlValue(string.Join(" ", value));

    public static string CleanXmlValue(this string value) => value.Replace("\t", "").Replace("\n", "").Replace("\r", "");
  }
}
