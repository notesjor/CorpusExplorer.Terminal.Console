using System;
using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Terminal.Console.Action.Abstract;

namespace CorpusExplorer.Terminal.Console.Action
{
  public class OutputAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels => new HashSet<string> {"conv", "convert"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var output = args.Last().Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries);
      if (output.Length != 2)
        return;
      var exporters = Configuration.AddonExporters.GetDictionary();
      if (!exporters.ContainsKey(output[0]))
        return;

      exporters[output[0]].Export(selection, output[1].Replace("\"", ""));
    }
  }
}