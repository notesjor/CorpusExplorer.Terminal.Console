using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CorpusExplorer.Terminal.Console.Web.Model.Request
{
  [Serializable]
  public class AddRequest
  {
    [JsonProperty("language")]
    public string Language { get; set; }

    [JsonProperty("documents")]
    public AddRequestDocument[] Documents { get; set; }

    public Dictionary<string, object>[] GetDocumentArray()
      => Documents.Select(x => x.GetDictionary()).ToArray();
  }
}
