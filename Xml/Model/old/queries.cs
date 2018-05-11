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
    
    private query[] queryField;
    
    private string processingField;
    
    /// <remarks/>
    [XmlElement("query")]
    public query[] query {
      get {
        return this.queryField;
      }
      set {
        this.queryField = value;
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