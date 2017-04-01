using CorpusExplorer.Port.RProgramming.Api.Exporter.Abstract;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;

namespace CorpusExplorer.Port.RProgramming.Api.Exporter
{
  public class ExporterCec5 : AbstractExporter
  {
    public override string FileExtension => ".cec5";

    public override void Export(Selection selection, string path)
      => selection.ToCorpus(new CorpusBuilderSingleFile()).Save(path, false);
  }
}