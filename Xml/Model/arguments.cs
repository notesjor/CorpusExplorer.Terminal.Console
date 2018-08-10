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
  public class arguments
  {
    private string[] argumentField;

    /// <remarks />
    [XmlElement("argument", DataType = "NCName")]
    public string[] argument
    {
      get => argumentField;
      set => argumentField = value;
    }
  }
}