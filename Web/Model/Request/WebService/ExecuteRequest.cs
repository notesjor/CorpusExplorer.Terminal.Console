using System;
using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request.WebService
{
  public class ExecuteRequest
  {
    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("arguments")]
    public string[] Arguments { get; set; }

    [JsonProperty("guids")]
    public Guid[] DocumentGuids { get; set; }
  }
}