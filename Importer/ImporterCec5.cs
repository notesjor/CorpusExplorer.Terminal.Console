using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer
{
  public class ImporterCec5 : AbstractImporter
  {
    public override string FileExtension => ".cec5";
    public override AbstractCorpusAdapter Import(string path) => CorpusAdapterSingleFile.Create(path);
  }
}