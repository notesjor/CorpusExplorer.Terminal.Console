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
  public partial class annotate {
    
    private object[] itemsField;
    
    private string languageField;
    
    private string taggerField;
    
    private string typeField;
    
    /// <remarks/>
    [XmlElement("directory", typeof(directory))]
    [XmlElement("file", typeof(file))]
    public object[] Items {
      get {
        return this.itemsField;
      }
      set {
        this.itemsField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string language {
      get {
        return this.languageField;
      }
      set {
        this.languageField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string tagger {
      get {
        return this.taggerField;
      }
      set {
        this.taggerField = value;
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