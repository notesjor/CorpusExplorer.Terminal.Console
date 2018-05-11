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
  public partial class session {
    
    private sources sourcesField;
    
    private queries queriesField;
    
    private task[] tasksField;
    
    /// <remarks/>
    public sources sources {
      get {
        return this.sourcesField;
      }
      set {
        this.sourcesField = value;
      }
    }
    
    /// <remarks/>
    public queries queries {
      get {
        return this.queriesField;
      }
      set {
        this.queriesField = value;
      }
    }
    
    /// <remarks/>
    [XmlArrayItem("task", IsNullable=false)]
    public task[] tasks {
      get {
        return this.tasksField;
      }
      set {
        this.tasksField = value;
      }
    }
  }
}