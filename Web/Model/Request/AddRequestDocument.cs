using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request
{
  [Serializable]
  [JsonObject("document")]
  public class AddRequestDocument
  {
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("meta")]
    public Dictionary<string, object> Metadata { get; set; }

    public Dictionary<string, object> GetDictionary()
    {
      if (Metadata.ContainsKey("Text"))
        Metadata["Text"] = Text;
      else
        Metadata.Add("Text", Text);

      return Metadata;
    }
  }
}