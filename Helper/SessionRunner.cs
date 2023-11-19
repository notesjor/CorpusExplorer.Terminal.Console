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
