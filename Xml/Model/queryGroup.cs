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
  public class queryGroup
  {
    private string nameField;

    private string operatorField;

    private query[] queryField;

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string name
    {
      get => nameField;
      set => nameField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string @operator
    {
      get => operatorField;
      set => operatorField = value;
    }

    /// <remarks />
    [XmlElement("query")]
    public query[] query
    {
      get => queryField;
      set => queryField = value;
    }
  }
}