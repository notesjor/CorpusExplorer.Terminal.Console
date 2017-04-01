using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Port.RProgramming.Api.Exporter.Abstract
{
  public abstract class AbstractExporter
  {
    public abstract string FileExtension { get; }
    public abstract void Export(Selection selection, string path);
  }
}