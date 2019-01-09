using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CorpusExplorer.Sdk.Action;
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
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Tagger.TreeTagger;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Web.Model.Request;
using CorpusExplorer.Terminal.Console.Xml.Extensions;
using CorpusExplorer.Terminal.Console.Xml.Model;
using CorpusExplorer.Terminal.WebOrbit.Model.Response;
using Newtonsoft.Json;
using Tfres;
using Tfres.Documentation;
using HttpRequest = Tfres.HttpRequest;
using HttpResponse = Tfres.HttpResponse;

namespace CorpusExplorer.Terminal.Console.Web
{
  public static class WebServiceDirect
  {
    private static string _availableActions;
    private static object _getAvailableActionsRouteLock = new object();
    private static AbstractTableWriter _writer;
    private static string _mime;
    private static string _documentation;
    private static string _url;

    public static void Run(AbstractTableWriter writer, int port)
    {
      _writer = writer;
      _mime = writer.MimeType;

      _url = $"http://127.0.0.1:{port}/";
      System.Console.Write($"SERVER {_url} ...");
      var s = new Server("127.0.0.1", port, DefaultRoute);
      s.AddEndpoint(HttpVerb.GET, "/actions/", GetAvailableActionsRoute);
      s.AddEndpoint(HttpVerb.POST, "/execute/", GetExecuteRoute);
      s.AddEndpoint(HttpVerb.POST, "/add/", GetAddRoute);

      if (!Directory.Exists("corpora"))
        Directory.CreateDirectory("corpora");

      System.Console.WriteLine("ready!");
    }

    private static HttpResponse GetAddRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<AddRequest>();
        if (er?.Documents == null || string.IsNullOrEmpty(er.Language))
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "no valid post-data"));

        var tagger = new SimpleTreeTagger();
        var available = new HashSet<string>(tagger.LanguagesAvailabel);
        if (!available.Contains(er.Language))
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, $"wrong language selected - use: {string.Join(", ", available)}"));

        var cleaner1 = new StandardCleanup();
        cleaner1.Input.Enqueue(er.GetDocumentArray());
        cleaner1.Execute();
        var cleaner2 = new RegexXmlMarkupCleanup { Input = cleaner1.Output };
        cleaner2.Execute();

        tagger.Input = cleaner2.Output;
        tagger.LanguageSelected = er.Language;
        tagger.Execute();
        var corpus = tagger.Output.First();
        if (corpus == null || corpus.CountDocuments == 0 || corpus.CountToken == 0)
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "tagging process failed"));

        corpus.Save($"corpora/{corpus.CorporaGuids.First()}.cec6", false);
        return new HttpResponse(req, false, 500, null, "application/json", $"{{ \"corpusId\": \"{corpus.CorporaGuids.First()}\" }}");
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, ex.Message));
      }
    }

    private static HttpResponse GetExecuteRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<DirectExecuteRequest>();
        if (er == null)
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "no valid post-data"));

        var aCheck = Configuration.GetConsoleAction(er.Action);
        if (aCheck == null || !IsValidAction(er.Action))
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "action not available"));

        if (!File.Exists($"corpora/{er.CorpusId}.cec6"))
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "corpus not (longer) available"));

        var corpus = CorpusAdapterWriteDirect.Create($"corpora/{er.CorpusId}.cec6");
        if (corpus == null)
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "corpus not (longer) available"));

        var selection = corpus.ToSelection();
        var a = new ClusterAction();
        var args = new List<string>
        {
          "XSGUID::TEXT",
          er.Action
        };
        if (er.Arguments != null && er.Arguments.Length > 0)
          args.AddRange(er.Arguments);

        using (var ms = new MemoryStream())
        {
          var writer = _writer.Clone(ms);
          a.Execute(selection, args.ToArray(), writer);
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
            Items = Configuration.AddonConsoleActions
                                 .Where(action => IsValidAction(action.Action))
                                 .Select(action =>
                                           new AvailableActionsResponse.
                                             AvailableActionsResponseItem
                                           {
                                             action = action.Action,
                                             description = action.Description
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

    private static bool IsValidAction(string action)
    {
      return !action.Contains("cluster") || action != "convert" || action != "query";
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
      return new HttpResponse(req, true, 200, null, "application/json", Documentation);
    }

    private static string Documentation
    {
      get
      {
        if (_documentation != null)
          return _documentation;

        var doc = new SericeDocumentation
        {
          Description = "CorpusExplorer-Endpoint (Version 1.0.0)",
          Url = _url,
          Endpoints = new[]
          {
            new ServiceEndpoint
            {
              Url = $"{_url}add/",
              AllowedVerbs = new[] {"POST"},
              Arguments = null,
              Description = "Adds/analyzes a new corpus",
              ReturnValue = new[]
              {
                new ServiceParameter
                  {Name = "language", Type = "string", Description = "the language of all documents"},
                new ServiceParameter
                  {Name = "documents", Type = "array of objects", Description = "text = document-text / meta = key/value dictionary - example: {\"text\":\"annotate this text\",\"meta\":{\"Author\":\"Jan\",\"Integer\":5,\"Date\":\"2019-01-08T21:32:01.0194747+01:00\"}}"},
              }
            },
            new ServiceEndpoint
            {
              Url = $"{_url}actions/",
              AllowedVerbs = new[] {"GET"},
              Arguments = null,
              Description = $"Shows all available Actions for {_url}execute/",
              ReturnValue = new[]
              {
                new ServiceParameter
                  {Name = "action", Type = "string", Description = "The name of the action"},
                new ServiceParameter
                  {Name = "description", Type = "string", Description = "Short description - action and parameter"},
              }
            },
            new ServiceEndpoint
            {
              Url = $"{_url}execute/",
              AllowedVerbs = new[] {"POST"},
              Arguments = new[]
              {
                new ServiceArgument
                  {Name = "corpusId", Type = "string", Description = $"the id of the corpus you added via {_url}add/", IsRequired = true},
                new ServiceArgument
                  {Name = "action", Type = "string", Description = "name of the action to execute", IsRequired = true},
                new ServiceArgument
                  {Name = "arguments", Type = "key-value", Description = "example: {'key1':'value1', 'key2':'value2', 'key3':'value3'}", IsRequired = true}
              },
              Description = $"Shows all available Actions for {_url}execute/",
              ReturnValue = new[]
              {
                new ServiceParameter
                  {Name = "table", Type = "table (rows > array of objects)", Description = "execution result"},
              }
            },
          }
        };

        _documentation = JsonConvert.SerializeObject(doc);
        return _documentation;
      }
    }
  }
}
