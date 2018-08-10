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
  public class output
  {
    private string formatField;

    private string valueField;

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string format
    {
      get => formatField;
      set => formatField = value;
    }

    /// <remarks />
    [XmlText(DataType = "anyURI")]
    public string Value
    {
      get => valueField;
      set => valueField = value;
    }
  }
}