namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class XmlValueHelper
  {
    public static string CleanXmlValue(this string[] value)
    {
      return CleanXmlValue(string.Join(" ", value));
    }

    public static string CleanXmlValue(this string value)
    {
      return value.Replace("\t", "").Replace("\n", "").Replace("\r", "");
    }
  }
}