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
  public class annotate
  {
    private object[] itemsField;

    private string languageField;

    private string taggerField;

    private string typeField;

    /// <remarks />
    [XmlElement("directory", typeof(directory))]
    [XmlElement("file", typeof(file))]
    public object[] Items
    {
      get => itemsField;
      set => itemsField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string language
    {
      get => languageField;
      set => languageField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string tagger
    {
      get => taggerField;
      set => taggerField = value;
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