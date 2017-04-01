using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer.Abstract
{
  public abstract class AbstractImporter
  {
    public abstract string FileExtension { get; }
    public abstract AbstractCorpusAdapter Import(string path);
  }
}