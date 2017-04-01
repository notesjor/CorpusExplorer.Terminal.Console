using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Port.RProgramming.Api.Exporter;
using CorpusExplorer.Port.RProgramming.Api.Exporter.Abstract;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class ConvertAction : AbstractAction
  {
    private static readonly AbstractExporter[] _exporter =
    {
      new ExporterCec5(),
      new ExporterCec6(),
      new ExporterDtaBf(),
      new ExporterWeblicht()
    };

    protected override HashSet<string> MatchActionLabels => new HashSet<string>{"conv", "convert"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var output = args.Last();
      _exporter.FirstOrDefault(x => output.ToLower().EndsWith(x.FileExtension))?.Export(selection, output);
    }
  }
}
