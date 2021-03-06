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
  public partial class query {
    
    private string nameField;
    
    private string[] textField;
    
    /// <remarks/>
    [XmlAttribute(DataType="NCName")]
    public string name {
      get {
        return this.nameField;
      }
      set {
        this.nameField = value;
      }
    }
    
    /// <remarks/>
    [XmlText()]
    public string[] Text {
      get {
        return this.textField;
      }
      set {
        this.textField = value;
      }
    }
  }
}