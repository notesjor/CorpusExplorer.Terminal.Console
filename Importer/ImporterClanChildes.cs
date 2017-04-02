using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer
{
  public class ImporterClanChildes : AbstractImporter
  {
    public override bool Match(string path) { return path.ToLower().EndsWith(".cex"); }

    public override AbstractCorpusAdapter Import(string path)
    {
      var importer = new ImporterClanChildes();
      return importer.Import(path);
    }
  }
}