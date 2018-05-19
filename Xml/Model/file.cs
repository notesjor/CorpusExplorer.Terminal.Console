﻿using System;
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
  public class file
  {
    private bool deleteField;

    private bool deleteFieldSpecified;

    private string valueField;

    /// <remarks />
    [XmlAttribute]
    public bool delete
    {
      get => deleteField;
      set => deleteField = value;
    }

    /// <remarks />
    [XmlIgnore]
    public bool deleteSpecified
    {
      get => deleteFieldSpecified;
      set => deleteFieldSpecified = value;
    }

    /// <remarks />
    [XmlText(DataType = "anyURI")]
    public string Value
    {
      get => valueField;
      set => valueField = value;
    }
  }
}