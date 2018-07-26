using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [XmlType(AnonymousType=true)]
  [XmlRoot(Namespace="", IsNullable=false)]
  public partial class queries {
    
    private object[] itemsField;
    
    private string processingField;
    
    /// <remarks/>
    [XmlElement("query", typeof(query))]
    [XmlElement("queryBuilder", typeof(queryBuilder))]
    [XmlElement("queryGroup", typeof(queryGroup))]
    public object[] Items {
      get {
        return this.itemsField;
      }
      set {
        this.itemsField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string processing {
      get {
        return this.processingField;
      }
      set {
        this.processingField = value;
      }
    }
  }
}