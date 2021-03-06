﻿using System.Xml.Serialization;

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [XmlType(AnonymousType=true)]
  [XmlRoot(Namespace="", IsNullable=false)]
  public partial class meta {
    
    private string keyField;
    
    private string valueField;
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string key {
      get {
        return this.keyField;
      }
      set {
        this.keyField = value;
      }
    }
    
    /// <remarks/>
    [XmlText(DataType="NMTOKEN")]
    public string Value {
      get {
        return this.valueField;
      }
      set {
        this.valueField = value;
      }
    }
  }
}