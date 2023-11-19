namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute("file", IsNullable = false)]
  public partial class myFile
  {

    private bool deleteField;

    private bool deleteFieldSpecified;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool delete
    {
      get { return this.deleteField; }
      set { this.deleteField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool deleteSpecified
    {
      get { return this.deleteFieldSpecified; }
      set { this.deleteFieldSpecified = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute(DataType = "anyURI")]
    public string Value
    {
      get { return this.valueField; }
      set { this.valueField = value; }
    }
  }
}