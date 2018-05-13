namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
  public partial class head {
    
    private meta[] metaField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("meta")]
    public meta[] meta {
      get {
        return this.metaField;
      }
      set {
        this.metaField = value;
      }
    }
  }
}