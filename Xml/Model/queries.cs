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
  public class queries
  {
    private object[] itemsField;

    /// <remarks />
    [XmlElement("query", typeof(query))]
    [XmlElement("queryBuilder", typeof(queryBuilder))]
    [XmlElement("queryGroup", typeof(queryGroup))]
    public object[] Items
    {
      get => itemsField;
      set => itemsField = value;
    }
  }
}