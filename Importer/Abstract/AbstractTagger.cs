using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using CorpusExplorer.Core.DocumentProcessing.Scraper.Docx;
using CorpusExplorer.Core.DocumentProcessing.Scraper.Html;
using CorpusExplorer.Core.DocumentProcessing.Scraper.Rtf;
using CorpusExplorer.Core.DocumentProcessing.Scraper.Txt;
using CorpusExplorer.Sdk.Extern.Json.Twitter;
using CorpusExplorer.Sdk.Extern.Xml.Exmaralda;
using CorpusExplorer.Sdk.Extern.Xml.Tei.Dwds;
using CorpusExplorer.Sdk.Extern.Xml.Tiger;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Scraper.Abstract;

namespace CorpusExplorer.Port.RProgramming.Api.Importer.Abstract
{
  public abstract class AbstractTagger : AbstractImporter
  {
    private Dictionary<string, AbstractScraper> _scraper = new Dictionary<string, AbstractScraper>
    {
      {"txt", new TxtScraper()},
      {"docx", new SimpleDocxDocumentScraper()},
      {"rtf", new SimpleRtfDocumentScraper()},
      {"html", new SimpleHtmlDocumentScraper()},
      {"exb", new ExmaraldaExbScraper()},
      {"tiger", new TigerScraper()},
      {"dwds", new DwdsTeiScraper()},
      {"clarin", new DwdsTeiScraper()},
      {"twitter", new TwitterScraper()}
    };

    public override AbstractCorpusAdapter Import(string path)
    {
      var split = path.Split(new[] {":", "|"}, StringSplitOptions.RemoveEmptyEntries);
      if (split.Length != 4)
        return null;

      var filetype = split[1];
      var language = split[2];
      var directory = split[3];

      if (!_scraper.ContainsKey(filetype))
        return null;

      // Stellt alle Dateien aus directory in den Scraper-Warteschlange
      var files = Directory.GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly);
      foreach (var file in files)
        _scraper[filetype].Input.Enqueue(file);
      _scraper[filetype].Execute();

      // Reicht die Scraper-Ergebnisse an den Cleaner weiter (bereinigt eine nicht lesbare Zeichen)
      var cleanup = new StandardCleanup {Input = _scraper[filetype].Output};
      cleanup.Execute();

      return Annotate(language, cleanup.Output);
    }

    protected abstract AbstractCorpusAdapter Annotate(string language, ConcurrentQueue<Dictionary<string, object>> docs);
  }
}