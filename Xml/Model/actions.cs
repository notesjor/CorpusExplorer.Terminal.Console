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
  public partial class actions {
    
    private action[] actionField;
    
    private string modeField;
    
    /// <remarks/>
    [XmlElement("action")]
    public action[] action {
      get {
        return this.actionField;
      }
      set {
        this.actionField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string mode {
      get {
        return this.modeField;
      }
      set {
        this.modeField = value;
      }
    }
  }
}