using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer
{
  public class ImporterCec6 : AbstractImporter
  {
    public override bool Match(string path) { return path.ToLower().EndsWith(".cec6"); }
    public override AbstractCorpusAdapter Import(string path) => CorpusAdapterWriteDirect.Create(path);
  }
}