namespace CorpusExplorer.Terminal.Console.Web.Model.Response.Abstract
{
  public abstract class AbstractResponse
  {
    public string error { get; set; }
    public bool hasError => !string.IsNullOrEmpty(error);
  }
}