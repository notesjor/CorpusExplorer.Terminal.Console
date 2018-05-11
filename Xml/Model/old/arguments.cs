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
  public partial class arguments {
    
    private string[] argumentField;
    
    /// <remarks/>
    [XmlElement("argument", DataType="NCName")]
    public string[] argument {
      get {
        return this.argumentField;
      }
      set {
        this.argumentField = value;
      }
    }
  }
}