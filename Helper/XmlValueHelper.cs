namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class XmlValueHelper
  {
    public static string CleanXmlValue(this string[] value)
    {
      return value == null ? string.Empty : CleanXmlValue(string.Join(" ", value));
    }

    public static string CleanXmlValue(this string value)
    {
      return value.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim();
    }
  }
}