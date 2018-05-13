namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
  public partial class queries {
    
    private object[] itemsField;
    
    private string processingField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("query", typeof(query))]
    [System.Xml.Serialization.XmlElementAttribute("queryBuilder", typeof(queryBuilder))]
    [System.Xml.Serialization.XmlElementAttribute("queryGroup", typeof(queryGroup))]
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