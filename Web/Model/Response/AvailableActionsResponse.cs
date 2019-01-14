using CorpusExplorer.Terminal.Console.Web.Model.Response.Abstract;

namespace CorpusExplorer.Terminal.Console.Web.Model.Response
{
  public class AvailableActionsResponse : AbstractResponse
  {
    public AvailableActionsResponseItem[] Items { get; set; }

    public class AvailableActionsResponseItem
    {
      public string action { get; set; }
      public string description { get; set; }
    }
  }
}