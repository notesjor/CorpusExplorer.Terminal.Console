/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class import {
    
  private object[] itemsField;
    
  private string typeField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("directory", typeof(directory))]
  [System.Xml.Serialization.XmlElementAttribute("file", typeof(string), DataType="anyURI")]
  public object[] Items {
    get {
      return this.itemsField;
    }
    set {
      this.itemsField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
  public string type {
    get {
      return this.typeField;
    }
    set {
      this.typeField = value;
    }
  }
}