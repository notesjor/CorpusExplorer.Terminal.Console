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
    
    private actions actionsField;
    
    private bool overrideField;
    
    private bool overrideFieldSpecified;
    
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
    public actions actions {
      get {
        return this.actionsField;
      }
      set {
        this.actionsField = value;
      }
    }
    
    /// <remarks/>
    [XmlAttribute()]
    public bool @override {
      get {
        return this.overrideField;
      }
      set {
        this.overrideField = value;
      }
    }
    
    /// <remarks/>
    [XmlIgnore()]
    public bool overrideSpecified {
      get {
        return this.overrideFieldSpecified;
      }
      set {
        this.overrideFieldSpecified = value;
      }
    }
  }
}