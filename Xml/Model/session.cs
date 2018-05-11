/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
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
  [System.Xml.Serialization.XmlArrayItemAttribute("task", IsNullable=false)]
  public task[] tasks {
    get {
      return this.tasksField;
    }
    set {
      this.tasksField = value;
    }
  }
}