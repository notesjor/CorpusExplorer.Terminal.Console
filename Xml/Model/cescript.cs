namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
  public partial class cescript {
    
    private meta[] headField;
    
    private session[] sessionsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("meta", IsNullable=false)]
    public meta[] head {
      get {
        return this.headField;
      }
      set {
        this.headField = value;
      }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("session", IsNullable=false)]
    public session[] sessions {
      get {
        return this.sessionsField;
      }
      set {
        this.sessionsField = value;
      }
    }
  }
}