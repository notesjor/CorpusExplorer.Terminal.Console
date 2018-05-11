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
  public partial class output {
    
    private string formatField;
    
    private string valueField;
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string format {
      get {
        return this.formatField;
      }
      set {
        this.formatField = value;
      }
    }
    
    /// <remarks/>
    [XmlText(DataType="anyURI")]
    public string Value {
      get {
        return this.valueField;
      }
      set {
        this.valueField = value;
      }
    }
  }
}