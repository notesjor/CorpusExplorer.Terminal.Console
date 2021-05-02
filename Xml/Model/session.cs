namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class session
  {

    private sources sourcesField;

    private queries queriesField;

    private templates templatesField;

    private actions actionsField;

    private bool overrideField;

    private bool overrideFieldSpecified;

    /// <remarks/>
    public sources sources
    {
      get { return this.sourcesField; }
      set { this.sourcesField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("queries", typeof(queries), IsNullable = false)]
    public queries queries
    {
      get { return this.queriesField; }
      set { this.queriesField = value; }
    }

    /// <remarks/>
    public templates templates
    {
      get { return this.templatesField; }
      set { this.templatesField = value; }
    }

    /// <remarks/>
    public actions actions
    {
      get { return this.actionsField; }
      set { this.actionsField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool @override
    {
      get { return this.overrideField; }
      set { this.overrideField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool overrideSpecified
    {
      get { return this.overrideFieldSpecified; }
      set { this.overrideFieldSpecified = value; }
    }
  }
}