﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// Dieser Quellcode wurde automatisch generiert von xsd, Version=4.8.3928.0.
// 

namespace CorpusExplorer.Terminal.Console.Xml.Model
{
  /// <remarks/>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
  [System.SerializableAttribute()]
  [System.Diagnostics.DebuggerStepThroughAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class cescript
  {

    private meta[] headField;

    private sessions sessionsField;

    private string versionField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("meta", IsNullable = false)]
    public meta[] head
    {
      get { return this.headField; }
      set { this.headField = value; }
    }

    /// <remarks/>
    public sessions sessions
    {
      get { return this.sessionsField; }
      set { this.sessionsField = value; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType = "NMTOKEN")]
    public string version
    {
      get { return this.versionField; }
      set { this.versionField = value; }
    }
  }
}