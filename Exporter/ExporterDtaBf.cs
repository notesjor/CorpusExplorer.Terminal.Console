using CorpusExplorer.Port.RProgramming.Api.Exporter.Abstract;
using CorpusExplorer.Sdk.Extern.Xml.Dta.Tcf;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Exporter
{
  public class ExporterDtaBf : AbstractExporter
  {
    public override string FileExtension => ".tcf.xml";

    public override void Export(Selection selection, string path)
    {
      var exporter = new DtaExporter();
      exporter.Export(selection, path);
    }
  }
}