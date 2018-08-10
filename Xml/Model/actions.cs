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
  public class actions
  {
    private action[] actionField;

    private string modeField;

    /// <remarks />
    [XmlElement("action")]
    public action[] action
    {
      get => actionField;
      set => actionField = value;
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