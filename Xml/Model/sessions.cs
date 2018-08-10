using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks />
  [GeneratedCode("xsd", "4.6.1055.0")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public class sessions
  {
    private string modeField;

    private session[] sessionField;

    /// <remarks />
    [XmlElement("session")]
    public session[] session
    {
      get => sessionField;
      set => sessionField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string mode
    {
      get => modeField;
      set => modeField = value;
    }
  }
}