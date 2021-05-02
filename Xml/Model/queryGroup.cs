namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class queryGroup
  {

    private query[] queryField;

    private string nameField;

    private string operatorField;

    private string parentField;

    private string prefixField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("query")]
    public query[] query
    {
      get { return this.queryField; }
      set { this.queryField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string name
    {
      get { return this.nameField; }
      set { this.nameField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string @operator
    {
      get { return this.operatorField; }
      set { this.operatorField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string parent
    {
      get { return this.parentField; }
      set { this.parentField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string prefix
    {
      get { return this.prefixField; }
      set { this.prefixField = value; }
    }
  }
}