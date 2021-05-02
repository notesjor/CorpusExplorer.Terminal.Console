using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using CorpusExplorer.Terminal.Console.Xml.Model;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class CeScriptHelper
  {
    /// <summary>
    ///   Lade/Deserialisere das CeScrpit
    /// </summary>
    /// <param name="path">Pfad</param>
    /// <param name="scriptFilename">Gibt den Dateinamen ohne Erweiterung zurück</param>
    /// <returns>CeScript</returns>
    public static cescript LoadCeScript(string path, out string scriptFilename)
    {
      var script = Load(path, out scriptFilename);
      foreach (var s in script.sessions.session)
      {
        if (s?.templates == null) 
          continue;

        foreach (var t in s.templates)
        {
          var replaces = t.variable.ToDictionary(v => $"{{{v.key}}}", v => v.value);
          var template = File.ReadAllText(t.src, Encoding.UTF8);
          template = replaces.Aggregate(template, (current, replace) => current.Replace(replace.Key, replace.Value));

          using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(template)))
          {
            var se = new XmlSerializer(typeof(actions));
            var actions = se.Deserialize(ms) as actions;

            var tmp = new List<action>();
            if (s.actions?.action != null && s.actions.action.Length > 0)
              tmp.AddRange(s.actions.action);
            if (actions?.action != null && actions.action.Length > 0)
              tmp.AddRange(actions.action);

            if (s.actions == null)
              s.actions = new actions { action = tmp.ToArray(), mode = actions?.mode };
            else
              s.actions.action = tmp.ToArray();
          }
        }
      }

      return script;
    }

    private static cescript Load(string path, out string scriptFilename)
    {
      cescript script = null;
      scriptFilename = Path.GetFileNameWithoutExtension(path);

      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        var se = new XmlSerializer(typeof(cescript));
        script = se.Deserialize(fs) as cescript;
      }

      return script;
    }
  }
}