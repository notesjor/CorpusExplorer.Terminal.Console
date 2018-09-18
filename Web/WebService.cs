using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Xml.Extensions;
using CorpusExplorer.Terminal.Console.Xml.Model;
using CorpusExplorer.Terminal.WebOrbit.Model.Request;
using CorpusExplorer.Terminal.WebOrbit.Model.Response;
using Newtonsoft.Json;
using Tfres;
using HttpRequest = Tfres.HttpRequest;
using HttpResponse = Tfres.HttpResponse;

namespace CorpusExplorer.Terminal.Console.Web
{
  public static class WebService
  {
    private static string _availableActions;
    private static object _getAvailableActionsRouteLock = new object();
    private static AbstractTableWriter _writer;
    private static AbstractCorpusAdapter _corpus;
    private static Dictionary<string, string> _cache = new Dictionary<string, string>();
    private static string _mime;

    public static void Run(AbstractTableWriter writer, int port, string file)
    {
      if (file.StartsWith("FILE:"))
      {
        System.Console.WriteLine("INIT WebService (mode: script)");
        _corpus = GetCorpusFromScript(file.Replace("FILE:", ""));
      }
      else
      {
        System.Console.WriteLine("INIT WebService (mode: file)");
        System.Console.Write($"LOAD: {file}...");
        _corpus = CorpusLoadHelper.LoadCorpus(file);
        System.Console.WriteLine("ok!");
      }
      _writer = writer;
      _mime = writer.MimeType;

      System.Console.Write($"SERVER http://127.0.0.1:{port}/ ...");
      var s = new Server("127.0.0.1", port, DefaultRoute);
      s.AddEndpoint(HttpVerb.GET, "/actions/", GetAvailableActionsRoute);
      s.AddEndpoint(HttpVerb.POST, "/execute/", GetExecuteRoute);
      System.Console.WriteLine("ready!");
    }

    private static HttpResponse GetExecuteRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<ExecuteRequest>();
        if (er == null)
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "no valid post-data"));

        if (_cache.Count > 0 && (er.DocumentGuids == null || er.DocumentGuids.Length == 0))
        {
          var key = GetCacheKey(er.Action, er.Arguments);
          if(_cache.ContainsKey(key))
            return new HttpResponse(req, true, 200, null, _mime, _cache[key]);
        }

        var a = Configuration.GetConsoleAction(er.Action);
        if (a == null)
          return null;

        var selection = _corpus.ToSelection();
        if (er.DocumentGuids != null && er.DocumentGuids.Length > 0)
          selection = selection.CreateTemporary(er.DocumentGuids);

        using (var ms = new MemoryStream())
        {
          var writer = _writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          return new HttpResponse(req, true, 200, null, _mime, Encoding.UTF8.GetString(ms.ToArray()));
        }
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, ex.Message));
      }
    }

    private static HttpResponse GetAvailableActionsRoute(HttpRequest req)
    {
      lock (_getAvailableActionsRouteLock)
        try
        {
          if (_availableActions != null)
            return new HttpResponse(req, true, 200, null, _mime, _availableActions);

          var res = new AvailableActionsResponse
          {
            Items = Configuration.AddonConsoleActions.Select(action =>
                                                               new AvailableActionsResponse.
                                                                 AvailableActionsResponseItem
                                                               {
                                                                 action = action.Action,
                                                                 describtion = action.Description
                                                               }).ToArray()
          };
          _availableActions = JsonConvert.SerializeObject(res);

          return new HttpResponse(req, true, 200, null, _mime, _availableActions);
        }
        catch (Exception ex)
        {
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, ex.Message));
        }
    }

    private static string WriteError(AbstractTableWriter prototype, string message)
    {
      using (var ms = new MemoryStream())
      {
        var writer = prototype.Clone(ms);
        writer.WriteError(message);
        writer.Destroy(false);

        ms.Seek(0, SeekOrigin.Begin);
        return Encoding.UTF8.GetString(ms.ToArray());
      }
    }

    private static HttpResponse DefaultRoute(HttpRequest req)
    {
      return new HttpResponse(req, true, 200, null, "text/plain", "CorpusExplorer-Endpoint (Version 1.0.0)");
    }

    private static AbstractCorpusAdapter GetCorpusFromScript(string path)
    {
      string scriptFilename;
      cescript script;
      try
      {
        script = CeScriptHelper.LoadCeScript(path, out scriptFilename);
      }
      catch (Exception ex)
      {
        System.Console.WriteLine("E001: XML Parser Error");
        System.Console.WriteLine(ex.Message);
        throw ex;
      }

      if (script.sessions.session.Length != 1)
        throw new ArgumentOutOfRangeException("E101: WebService-Mode can only handle one session");

      var session = script.sessions.session.Single();
      if (session.queries?.Items != null && session.queries.Items.Length > 0)
        throw new ArgumentOutOfRangeException("E102: WebService-Mode can't process queries");

      System.Console.Write("FILE(S)...");
      var res = ReadSources(session.sources);
      var sel = res.ToSelection();
      System.Console.WriteLine("ok!");

      System.Console.Write("CACHE PRECALC...");
      foreach (var a in session.actions.action)
      {
        try
        {
          var action = Configuration.GetConsoleAction(a.type);
          if (action == null)
            continue;

          ExecuteAction(action, a, sel);
        }
        catch
        {
          // ignore
        }
      }
      System.Console.WriteLine("ok!");

      return res;
    }

    private static void ExecuteAction(IAction action, action a, Selection selection)
    {
      try
      {
        // WebService a.mode ist immer merge - im Vergleich zu XmlScriptProcessor (!string.IsNullOrEmpty(a.mode) && a.mode == "merge")
        // query und convert können nicht vom WebService verarbeitet werden.
        if (a.type == "query" || a.type == "convert")
          return;

        var key = GetCacheKey(a.type, a.arguments);
        if (_cache.ContainsKey(key))
          return;

        // Kopie des TableWriter, um eine parallele Verarbeitung zu ermöglichen.
        using (var ms = new MemoryStream())
        using (var bs = new BufferedStream(ms))
        {
          var format = _writer.Clone(bs);
          action.Execute(selection, a.arguments, format);
          format.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          _cache.Add(key, Encoding.UTF8.GetString(ms.ToArray()));
        }
      }
      catch
      {
        // ignore
      }
    }

    private static string GetCacheKey(string action, params string[] parameters)
    {
      return parameters == null ? action : string.Join("|", action, parameters);
    }

    /// <summary>
    ///   Liest die gewünschten Korpusquellen ein
    /// </summary>
    /// <param name="sources">Quellen</param>
    /// <returns>Corpus</returns>
    private static AbstractCorpusAdapter ReadSources(sources sources)
    {
      var res = CorpusExplorerEcosystem.InitializeMinimal();

      // Wenn zu annotierendes Material vorhanden ist, dann lese dieses ein.
      if (sources.annotate().Any())
      {
        var scrapers = Configuration.AddonScrapers.GetDictionary();
        var taggers = Configuration.AddonTaggers.GetDictionary();

        foreach (var annotate in sources.annotate())
          try
          {
            if (!scrapers.ContainsKey(annotate.type))
              continue;
            if (!taggers.ContainsKey(annotate.tagger))
              continue;

            // Extrahiere und bereinige die Dokumente
            var scraper = scrapers[annotate.type];
            scraper.Input.Enqueue(SearchFiles(annotate.Items));
            scraper.Execute();
            var cleaner1 = new StandardCleanup { Input = scraper.Output };
            cleaner1.Execute();
            var cleaner2 = new RegexXmlMarkupCleanup { Input = cleaner1.Output };
            cleaner2.Execute();

            // Annotiere das Textmaterial
            var tagger = taggers[annotate.tagger];
            tagger.LanguageSelected = annotate.language;
            tagger.Input = cleaner2.Output;
            tagger.Execute();

            foreach (var corpus in tagger.Output)
              res.Add(corpus);
          }
          catch (Exception ex)
          {
            throw ex;
          }
      }

      // Wenn Import-Quellen vorhanden sind, dann lese diese ein.
      if (sources.import().Any())
      {
        var importers = Configuration.AddonImporters.GetDictionary();
        foreach (var import in sources.import())
          try
          {
            if (!importers.ContainsKey(import.type))
              continue;

            foreach (var corpus in importers[import.type].Execute(SearchFiles(import.Items)))
              res.Add(corpus);
          }
          catch (Exception ex)
          {
            throw ex;
          }
      }

      return res.ToCorpus();
    }

    private static IEnumerable<string> SearchFiles(object[] annotateItems)
    {
      var res = new List<string>();

      foreach (var item in annotateItems)
        try
        {
          switch (item)
          {
            case file i:
              res.Add(i.Value);
              break;
            case directory i:
              var files = Directory.GetFiles(i.Value, i.filter, SearchOption.TopDirectoryOnly);
              res.AddRange(files);
              break;
          }
        }
        catch (Exception ex)
        {
          throw ex;
        }

      return res;
    }
  }
}
