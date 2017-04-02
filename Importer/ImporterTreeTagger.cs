using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Core.DocumentProcessing.Tagger.TreeTagger;
using CorpusExplorer.Core.DocumentProcessing.Tokenizer;
using CorpusExplorer.Port.RProgramming.Api.Importer.Abstract;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Builder;

namespace CorpusExplorer.Port.RProgramming.Api.Importer
{
  public class ImporterTreeTagger : AbstractTagger
  {
    private Dictionary<string, string> _langauges = new Dictionary<string, string>
    {
      {"german", "Deutsch"},
      {"english", "Englisch"},
      {"french", "Französisch"},
      {"italian", "Italienisch"},
      {"dutch", "Niederländisch"},
      {"spanish", "Spanisch"},
      {"polish", "Polnisch"}
    };

    public override bool Match(string path) { return path.StartsWith("treetagger:"); }

    protected override AbstractCorpusAdapter Annotate(string language, ConcurrentQueue<Dictionary<string, object>> docs)
    {
      if (!_langauges.ContainsKey(language))
        return null;

      var tagger = new SimpleTreeTagger
      {
        LanguageSelected = _langauges[language],
        Input = docs,
        CorpusBuilder = new CorpusBuilderSingleFile(),
        Tokenizer = new TreeTaggerTokenizer()
      };

      tagger.Execute();
      return tagger.Output.First();
    }
  }
}