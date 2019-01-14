using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Action;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Cleanup;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Tagger.TreeTagger;
using CorpusExplorer.Terminal.Console.Web.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model.Request.WebServiceDirect;
using CorpusExplorer.Terminal.Console.Web.Model.Response;
using Newtonsoft.Json;
using Tfres;
using Tfres.Documentation;

namespace CorpusExplorer.Terminal.Console.Web
{
  public class WebServiceDirect : AbstractWebService
  {
    public WebServiceDirect(AbstractTableWriter writer, int port) : base(writer, port)
    {
    }

    private HttpResponse GetAddRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<AddRequest>();
        if (er?.Documents == null || string.IsNullOrEmpty(er.Language))
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "no valid post-data"));

        return UseTagger(req, er.Language, er.GetDocumentArray(), true);
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, ex.Message));
      }
    }

    private HttpResponse UseTagger(HttpRequest req, string language, Dictionary<string, object>[] docs,
                                   bool enableCleanup)
    {
      var tagger = new SimpleTreeTagger();
      var available = new HashSet<string>(tagger.LanguagesAvailabel);
      if (!available.Contains(language))
        return new HttpResponse(req, false, 500, null, Mime,
                                WriteError(Writer, $"wrong language selected - use: {string.Join(", ", available)}"));

      if (enableCleanup)
      {
        var cleaner1 = new StandardCleanup();
        cleaner1.Input.Enqueue(docs);
        cleaner1.Execute();
        var cleaner2 = new RegexXmlMarkupCleanup {Input = cleaner1.Output};
        cleaner2.Execute();
        tagger.Input = cleaner2.Output;
      }
      else
      {
        tagger.Input.Enqueue(docs);
      }

      tagger.LanguageSelected = language;
      tagger.Execute();
      var corpus = tagger.Output.First();
      if (corpus == null || corpus.CountDocuments == 0 || corpus.CountToken == 0)
        return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "tagging process failed"));

      corpus.Save($"corpora/{corpus.CorporaGuids.First()}.cec6", false);
      return new HttpResponse(req, false, 500, null, "application/json",
                              $"{{ \"corpusId\": \"{corpus.CorporaGuids.First()}\" }}");
    }

    protected override HttpResponse GetExecuteRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<ExecuteRequest>();
        if (er == null)
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "no valid post-data"));

        var aCheck = Configuration.GetConsoleAction(er.Action);
        if (aCheck == null || !IsValidAction(er.Action))
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "action not available"));

        if (!File.Exists($"corpora/{er.CorpusId}.cec6"))
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "corpus not (longer) available"));

        var corpus = CorpusAdapterWriteDirect.Create($"corpora/{er.CorpusId}.cec6");
        if (corpus == null)
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "corpus not (longer) available"));

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
          var writer = Writer.Clone(ms);
          a.Execute(selection, args.ToArray(), writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          return new HttpResponse(req, true, 200, null, Mime, Encoding.UTF8.GetString(ms.ToArray()));
        }
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, ex.Message));
      }
    }    

    protected override Server ConfigureServer(Server server)
    {
      server.AddEndpoint(HttpVerb.POST, "/add/", GetAddRoute);

      if (!Directory.Exists("corpora"))
        Directory.CreateDirectory("corpora");

      return server;
    }

    protected override AvailableActionsResponse InitializeExportActionList()
      => new AvailableActionsResponse
      {
        Items = (from action in Configuration.AddonConsoleActions
                 where action.Action == "convert" || action.Action == "query"
                 select new AvailableActionsResponse.AvailableActionsResponseItem
                 {
                   action = action.Action,
                   description = action.Description
                 }).ToArray()
      };

    protected override AvailableActionsResponse InitializeExecuteActionList()
      => new AvailableActionsResponse
      {
        Items = (from action in Configuration.AddonConsoleActions
                 where !action.Action.Contains("cluster") || action.Action != "convert" || action.Action != "query"
                 select new AvailableActionsResponse.AvailableActionsResponseItem
                 {
                   action = action.Action,
                   description = action.Description
                 }).ToArray()
      };

    protected override SericeDocumentation GetDocumentation()
    {
      return new SericeDocumentation
      {
        Description = "CorpusExplorer-Endpoint (Version 1.0.0)",
        Url = Url,
        Endpoints = new[]
        {
          new ServiceEndpoint
          {
            Url = $"{Url}add/",
            AllowedVerbs = new[] {"POST"},
            Arguments = null,
            Description = "Adds/analyzes a new corpus",
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "language", Type = "string", Description = "the language of all documents"},
              new ServiceParameter
              {
                Name = "documents", Type = "array of objects",
                Description =
                  "text = document-text / meta = key/value dictionary - example: {\"text\":\"annotate this text\",\"meta\":{\"Author\":\"Jan\",\"Integer\":5,\"Date\":\"2019-01-08T21:32:01.0194747+01:00\"}}"
              }
            }
          },
          new ServiceEndpoint
          {
            Url = $"{Url}execute/",
            AllowedVerbs = new[] {"POST"},
            Arguments = new[]
            {
              new ServiceArgument
              {
                Name = "corpusId", Type = "string", Description = $"the id of the corpus you added via {Url}add/",
                IsRequired = true
              },
              new ServiceArgument
                {Name = "action", Type = "string", Description = "name of the action to execute", IsRequired = true},
              new ServiceArgument
              {
                Name = "arguments", Type = "key-value",
                Description = "example: {'key1':'value1', 'key2':'value2', 'key3':'value3'}", IsRequired = true
              }
            },
            Description = $"Shows all available Actions for {Url}execute/",
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "table", Type = "table (rows > array of objects)", Description = "execution result"}
            }
          }
        }
      };
    }
  }
}