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
  public class queryBuilder
  {
    private string nameField;

    private string parentField;

    private string prefixField;

    private string[] valueField;

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string name
    {
      get => nameField;
      set => nameField = value;
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string parent {
      get => parentField;
      set => parentField = value;
    }

    /// <remarks />
    [XmlAttribute]
    public string prefix
    {
      get => prefixField;
      set => prefixField = value;
    }

    /// <remarks />
    [XmlElement("value")]
    public string[] value
    {
      get => valueField;
      set => valueField = value;
    }
  }
}