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
  public class task
  {
    private string[] argumentsField;

    private output outputField;

    private string queryField;

    private string typeField;

    /// <remarks />
    [XmlArrayItem("argument", DataType = "NCName", IsNullable = false)]
    public string[] arguments
    {
      get => argumentsField;
      set => argumentsField = value;
    }

    /// <remarks />
    public output output
    {
      get => outputField;
      set => outputField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string query
    {
      get => queryField;
      set => queryField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string type
    {
      get => typeField;
      set => typeField = value;
    }
  }
}