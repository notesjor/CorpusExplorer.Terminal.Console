using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [XmlType(AnonymousType=true)]
  [XmlRoot(Namespace="", IsNullable=false)]
  public partial class tasks {
    
    private task[] taskField;
    
    /// <remarks/>
    [XmlElement("task")]
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