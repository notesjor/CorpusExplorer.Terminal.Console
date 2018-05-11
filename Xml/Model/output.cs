/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class output {
    
  private string formatField;
    
  private string valueField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
  public string format {
    get {
      return this.formatField;
    }
    set {
      this.formatField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlTextAttribute(DataType="anyURI")]
  public string Value {
    get {
      return this.valueField;
    }
    set {
      this.valueField = value;
    }
  }
}