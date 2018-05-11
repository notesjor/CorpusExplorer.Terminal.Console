/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class arguments {
    
  private string[] argumentField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("argument", DataType="NCName")]
  public string[] argument {
    get {
      return this.argumentField;
    }
    set {
      this.argumentField = value;
    }
  }
}