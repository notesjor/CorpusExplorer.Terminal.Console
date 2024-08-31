using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class SessionRunner
  {
    /// <summary>
    /// Importiert ein komplettes Verzeichnis und merged alle Korpora, die auf den Filter/Type passen und führt darauf die Session aus.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="importDirectory">Verzeichnis mit Korpora</param>
    /// <param name="filter">Dateifilter (z. B. *.cec6)</param>
    /// <param name="type">Import-Type</param>
    /// <param name="delete">Dateien löschen?</param>
    public static void Run(session session, string importDirectory, string filter, string type, bool delete)
    {
      Run(session, new import
      {
        Items = new[]
        {
          new directory
          {
            delete = delete,
            Value = importDirectory,
            filter = filter
          }
        },
        type = type
      });
    }

    /// <summary>
    /// Importiert eine einzelne Datei (Korpus) und führt darauf die Session aus.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="importFilePath">Pfad zum Korpus</param>
    /// <param name="type">Import-Type</param>
    /// <param name="delete">Löschen?</param>
    public static void Run(session session, string importFilePath, string type, bool delete)
    {
      Run(session, new import
      {
        Items = new[]
        {
          new myFile
          {
            delete = delete,
            Value = importFilePath
          }
        },
        type = type
      });
    }

    /// <summary>
    /// Wendet auf den Import die Session an.
    /// </summary>
    /// <param name="session">Session</param>
    /// <param name="import">Import (enthält ggf. Dateien/Ordner und Hinweise zur Verarbeitung)</param>
    public static void Run(session session, import import)
    {
      Run(new session
      {
        sources = new sources
        {
          processing = "synchron",
          parallel = "1",
          Items = new object[] { import }
        },
        queries = session.queries,
        templates = session.templates,
        actions = session.actions,
        overrideSpecified = session.overrideSpecified,
        @override = session.@override,
        InternalScriptPath = session.InternalScriptPath
      });
    }

    /// <summary>
    /// Führt eine Session aus.
    /// </summary>
    /// <param name="session">Session</param>
    public static void Run(session session)
    {
      string xml;
      using (var ms = new MemoryStream())
      using (var writer = new StreamWriter(ms, Encoding.UTF8))
      {
        var serializer = new XmlSerializer(typeof(session));
        serializer.Serialize(writer, session);
        xml = Encoding.UTF8.GetString(ms.ToArray());
      }

      var assembly = Assembly.GetExecutingAssembly().Location;

      // start process of the same exe and write to stdin
      var process = new System.Diagnostics.Process
      {
        StartInfo =
        {
          FileName = assembly.EndsWith(".dll") ? "dotnet" : assembly,
          Arguments = assembly.EndsWith(".dll") ? $"{assembly} --session" : "--session",
          UseShellExecute = false,
          CreateNoWindow = true,
          RedirectStandardInput = true,
          WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
          WorkingDirectory = Environment.CurrentDirectory,
        }
      };
      process.Start();
      process.StandardInput.Write(xml);
      process.StandardInput.Close();
      process.WaitForExit();
    }
  }
}
