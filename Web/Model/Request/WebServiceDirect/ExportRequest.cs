using System;
using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request.WebServiceDirect
{
  public class ExportRequest
  {
    [JsonProperty("corpusId")]
    public string CorpusId { get; set; }

    [JsonProperty("documentIds")]
    public Guid[] DocumentIds { get; set; }

    [JsonProperty("outputFormat")]
    public string OutputFormat { get; set; }

    [JsonProperty("action")]
    public string Action { get; set; }
  }
}