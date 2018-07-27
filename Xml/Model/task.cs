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
  public partial class task {
    
    private output outputField;
    
    private string[] argumentsField;

    private string modeField;
    
    private string queryField;
    
    private string typeField;
    
    /// <remarks/>
    public output output {
      get {
        return this.outputField;
      }
      set {
        this.outputField = value;
      }
    }
    
    /// <remarks/>
    [XmlArrayItem("argument", DataType="NCName", IsNullable=false)]
    public string[] arguments {
      get {
        return this.argumentsField;
      }
      set {
        this.argumentsField = value;
      }
    }

    /// <remarks/>
    [XmlAttributeAttribute(DataType="NCName")]
    public string mode {
      get {
        return this.modeField;
      }
      set {
        this.modeField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string query {
      get {
        return this.queryField;
      }
      set {
        this.queryField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string type {
      get {
        return this.typeField;
      }
      set {
        this.typeField = value;
      }
    }
  }
}