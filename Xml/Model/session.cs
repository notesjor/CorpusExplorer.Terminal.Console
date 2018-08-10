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
  public class session
  {
    private actions actionsField;

    private bool overrideField;

    private bool overrideFieldSpecified;

    private queries queriesField;

    private sources sourcesField;

    /// <remarks />
    public sources sources
    {
      get => sourcesField;
      set => sourcesField = value;
    }

    /// <remarks />
    public queries queries
    {
      get => queriesField;
      set => queriesField = value;
    }

    /// <remarks />
    public actions actions
    {
      get => actionsField;
      set => actionsField = value;
    }

    /// <remarks />
    [XmlAttribute]
    public bool @override
    {
      get => overrideField;
      set => overrideField = value;
    }

    /// <remarks />
    [XmlIgnore]
    public bool overrideSpecified
    {
      get => overrideFieldSpecified;
      set => overrideFieldSpecified = value;
    }
  }
}