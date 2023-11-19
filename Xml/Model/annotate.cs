namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class annotate
  {

    private object[] itemsField;

    private string languageField;

    private string taggerField;

    private string typeField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("directory", typeof(directory))]
    [System.Xml.Serialization.XmlElementAttribute("file", typeof(myFile))]
    public object[] Items
    {
      get { return this.itemsField; }
      set { this.itemsField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string language
    {
      get { return this.languageField; }
      set { this.languageField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string tagger
    {
      get { return this.taggerField; }
      set { this.taggerField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NCName")]
    public string type
    {
      get { return this.typeField; }
      set { this.typeField = value; }
    }
  }
}