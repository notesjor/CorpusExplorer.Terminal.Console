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
    private queries queriesField;

    private sources sourcesField;

    private task[] tasksField;

    /// <remarks />
    public queries queries
    {
      get => queriesField;
      set => queriesField = value;
    }

    /// <remarks />
    public sources sources
    {
      get => sourcesField;
      set => sourcesField = value;
    }

    /// <remarks />
    [XmlArrayItem("task", IsNullable = false)]
    public task[] tasks
    {
      get => tasksField;
      set => tasksField = value;
    }
  }
}