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
  public class action
  {
    private string[] argumentsField;

    private string modeField;

    private output outputField;

    private string queryField;

    private string typeField;

    /// <remarks />
    public output output
    {
      get => outputField;
      set => outputField = value;
    }

    /// <remarks />
    [XmlArrayItem("argument", DataType = "NCName", IsNullable = false)]
    public string[] arguments
    {
      get => argumentsField;
      set => argumentsField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string mode
    {
      get => modeField;
      set => modeField = value;
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