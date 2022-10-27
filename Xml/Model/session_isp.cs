using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  public partial class session
  {
    [XmlIgnore]
    public string InternalScriptPath { get; set; }
  }
}
