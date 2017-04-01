using CorpusExplorer.Port.RProgramming.Api.Exporter.Abstract;
using CorpusExplorer.Sdk.Extern.Xml.Weblicht;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Exporter
{
  public class ExporterWeblicht : AbstractExporter
  {
    public override string FileExtension => ".xml";

    public override void Export(Selection selection, string path)
    {
      var exporter = new WeblichtExporter();
      exporter.Export(selection, path);
    }
  }
}