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
  public class sources
  {
    private annotate[] annotateField;

    private import[] importField;

    private binary[] binaryField;

    private string processingField;

    /// <remarks />
    [XmlElement("annotate")]
    public annotate[] annotate
    {
      get => annotateField;
      set => annotateField = value;
    }

    /// <remarks />
    [XmlElement("import")]
    public import[] import
    {
      get => importField;
      set => importField = value;
    }

    [XmlElement("binary")]
    public binary[] binary
    {
      get => binaryField;
      set => binaryField = value;
    }

    /// <remarks />
    [XmlAttribute(DataType = "NCName")]
    public string processing
    {
      get => processingField;
      set => processingField = value;
    }
  }
}