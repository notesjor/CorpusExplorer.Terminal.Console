/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class sources {
    
  private annotate[] annotateField;
    
  private import[] importField;
    
  private string processingField;
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("annotate")]
  public annotate[] annotate {
    get {
      return this.annotateField;
    }
    set {
      this.annotateField = value;
    }
  }
    
  /// <remarks/>
  [System.Xml.Serialization.XmlElementAttribute("import")]
  public import[] import {
    get {
      return this.importField;
    }
    set {
      this.importField = value;
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