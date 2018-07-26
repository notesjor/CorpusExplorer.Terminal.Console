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
  public partial class queryGroup {
    
    private query[] queryField;
    
    private string nameField;
    
    private string operatorField;
    
    private string parentField;
    
    private string prefixField;
    
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
    public string name {
      get {
        return this.nameField;
      }
      set {
        this.nameField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string @operator {
      get {
        return this.operatorField;
      }
      set {
        this.operatorField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string parent {
      get {
        return this.parentField;
      }
      set {
        this.parentField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute()]
    public string prefix {
      get {
        return this.prefixField;
      }
      set {
        this.prefixField = value;
      }
    }
  }
}