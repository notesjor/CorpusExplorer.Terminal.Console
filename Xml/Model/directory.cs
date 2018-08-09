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
  public partial class directory {
    
    private bool deleteField;
    
    private bool deleteFieldSpecified;
    
    private string filterField;
    
    private string valueField;
    
    /// <remarks/>
    [XmlAttribute()]
    public bool delete {
      get {
        return this.deleteField;
      }
      set {
        this.deleteField = value;
      }
    }
    
    /// <remarks/>
    [XmlIgnore()]
    public bool deleteSpecified {
      get {
        return this.deleteFieldSpecified;
      }
      set {
        this.deleteFieldSpecified = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NMTOKEN")]
    public string filter {
      get {
        return this.filterField;
      }
      set {
        this.filterField = value;
      }
    }
    
    /// <remarks/>
    [XmlText(DataType="anyURI")]
    public string Value {
      get {
        return this.valueField;
      }
      set {
        this.valueField = value;
      }
    }
  }
}