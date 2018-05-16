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
  public class cescript
  {
    private meta[] headField;

    private session[] sessionsField;

    /// <remarks />
    [XmlArrayItem("meta", IsNullable = false)]
    public meta[] head
    {
      get => headField;
      set => headField = value;
    }

    /// <remarks />
    [XmlArrayItem("session", IsNullable = false)]
    public session[] sessions
    {
      get => sessionsField;
      set => sessionsField = value;
    }
  }
}