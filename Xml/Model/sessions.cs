namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
  public partial class sessions {
    
    private session[] sessionField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("session")]
    public session[] session {
      get {
        return this.sessionField;
      }
      set {
        this.sessionField = value;
      }
    }
  }
}