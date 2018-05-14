using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Terminal.Console.Action.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class OutputAction : AbstractAction
  {
    public override string Action => "convert";
    public override string Description => "convert - see help section [OUTPUT] for more information";

    public override void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var output = args.Last().Split(new[] { "#" }, StringSplitOptions.RemoveEmptyEntries);
      if (output.Length != 2)
        return;
      var exporters = Configuration.AddonExporters.GetDictionary();
      if (!exporters.ContainsKey(output[0]))
        return;

      var path = output[1].Replace("\"", "");
      var dir = Path.GetDirectoryName(path);
      if (dir != null && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      exporters[output[0]].Export(selection, path);
    }
  }
}