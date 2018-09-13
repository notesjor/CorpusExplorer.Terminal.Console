using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.WebOrbit.Model.Request
{
  public class ExecuteRequest
  {
    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("selection")]
    public string Selection { get; set; }

    [JsonProperty("arguments")]
    public string[] Arguments { get; set; }

    [JsonIgnore]
    public string CacheKey => Arguments == null ? $">{Action.Replace(">", "")}" : string.Join("|", Action.Replace(">", ""), Arguments);
  }
}
