using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks />
  [GeneratedCode("xsd", "4.6.1055.0")]
  [Serializable]
  [DebuggerStepThrough]
  [DesignerCategory("code")]
  [XmlType(AnonymousType = true)]
  [XmlRoot(Namespace = "", IsNullable = false)]
  public partial class queryGroup {
    
    private query[] queryField;
    
    private string nameField;
    
    private string operatorField;

    private string parentField;
    
    private string prefixField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("query")]
    public query[] query {
      get {
        return this.queryField;
      }
      set {
        this.queryField = value;
      }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string name {
      get {
        return this.nameField;
      }
      set {
        this.nameField = value;
      }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string @operator {
      get {
        return this.operatorField;
      }
      set {
        this.operatorField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="NCName")]
    public string parent {
      get {
        return this.parentField;
      }
      set {
        this.parentField = value;
      }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string prefix {
      get {
        return this.prefixField;
      }
      set {
        this.prefixField = value;
      }
    }
  }
}