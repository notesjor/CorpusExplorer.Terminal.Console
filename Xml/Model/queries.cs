/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class queries {
    
  private query[] queryField;
    
  private string processingField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("query")]
  public query[] query {
    get {
      return this.queryField;
    }
    set {
      this.queryField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
  public string processing {
    get {
      return this.processingField;
    }
    set {
      this.processingField = value;
    }
  }
}