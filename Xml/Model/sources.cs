namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class sources
  {

    private object[] itemsField;

    private string parallelField;

    private string processingField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("annotate", typeof(annotate))]
    [System.Xml.Serialization.XmlElementAttribute("import", typeof(import))]
    public object[] Items
    {
      get { return this.itemsField; }
      set { this.itemsField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "integer")]
    public string parallel
    {
      get { return this.parallelField; }
      set { this.parallelField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string processing
    {
      get { return this.processingField; }
      set { this.processingField = value; }
    }
  }
}