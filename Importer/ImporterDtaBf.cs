using System.Linq;
using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Extern.Xml.Dta.Tcf;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;

namespace CorpusExplorer.Port.RProgramming.Api.Importer
{
  public class ImporterDtaBf : AbstractImporter
  {
    public override string FileExtension => ".tcf.xml";

    public override AbstractCorpusAdapter Import(string path)
    {
      var importer = new DtaImporter {CorpusBuilder = new CorpusBuilderSingleFile()};
      return importer.Execute(new[] {path}).FirstOrDefault();
    }
  }
}