using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request.WebServiceDirect
{
  [Serializable]
  public class AddRequest
  {
    [JsonProperty("enableCleanup")]
    public bool EnableCleanup { get; set; }

    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("documents")]
    public AddRequestDocument[] Documents { get; set; }

    public Dictionary<string, object>[] GetDocumentArray()
      => Documents.Select(x => x.GetDictionary()).ToArray();
  }
}
