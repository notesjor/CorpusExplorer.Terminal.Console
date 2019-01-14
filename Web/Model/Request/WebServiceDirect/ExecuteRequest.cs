using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request.WebServiceDirect
{
  public class ExecuteRequest
  {
    [JsonProperty("corpusId")]
    public string CorpusId { get; set; }

    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("arguments")]
    public string[] Arguments { get; set; }
  }
}