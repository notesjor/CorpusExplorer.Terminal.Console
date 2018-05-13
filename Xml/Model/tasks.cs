namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
  public partial class tasks {
    
    private task[] taskField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("task")]
    public task[] task {
      get {
        return this.taskField;
      }
      set {
        this.taskField = value;
      }
    }
  }
}