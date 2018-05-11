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
  public partial class sources {
    
    private annotate[] annotateField;
    
    private import[] importField;
    
    /// <remarks/>
    [XmlElement("annotate")]
    public annotate[] annotate {
      get {
        return this.annotateField;
      }
      set {
        this.annotateField = value;
      }
    }
    
    /// <remarks/>
    [XmlElement("import")]
    public import[] import {
      get {
        return this.importField;
      }
      set {
        this.importField = value;
      }
    }
  }
}