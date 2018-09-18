using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
