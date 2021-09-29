namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class actions
  {

    private action[] actionField;

    private string modeField;

    private string parallelField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("action")]
    public action[] action
    {
      get { return this.actionField; }
      set { this.actionField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string mode
    {
      get { return this.modeField; }
      set { this.modeField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
    public string parallel
    {
      get { return this.parallelField; }
      set { this.parallelField = value; }
    }
  }
}