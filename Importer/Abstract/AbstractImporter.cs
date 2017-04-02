using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer.Abstract
{
  public abstract class AbstractImporter
  {
    public abstract bool Match(string path);
    public abstract AbstractCorpusAdapter Import(string path);
  }
}