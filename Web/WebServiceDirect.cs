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
using CorpusExplorer.Terminal.Console.Properties;
using CorpusExplorer.Terminal.Console.Web.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model;
using CorpusExplorer.Terminal.Console.Web.Model.Request.WebServiceDirect;
using Newtonsoft.Json;
using Tfres;
using Tfres.Documentation;

namespace CorpusExplorer.Terminal.Console.Web
{
  public class WebServiceDirect : AbstractWebService
  {
    private string _availableLanguages;

    public WebServiceDirect(AbstractTableWriter writer, string ip, int port, int timeout) : base(writer, ip, port, timeout)
    {
      _availableLanguages = JsonConvert.SerializeObject(InitializeAvailableLanguagesList());
    }

    private string[] InitializeAvailableLanguagesList()
    {
      return new SimpleTreeTagger().LanguagesAvailabel.ToArray();
    }

    private HttpResponse AvailableLanguagesRoute(HttpRequest arg)
    {
      try
      {
        return LimitExecuteTime(() => new HttpResponse(arg, true, 200, null, "application/json", _availableLanguages));
      }
      catch (Exception ex)
      {
        return WriteError(arg, ex.Message);
      }
    }

    protected override ActionFilter ExecuteActionFilter
      => new ActionFilter(false, "convert", "query", "cluster");

    private HttpResponse AddRoute(HttpRequest req)
    {
      try
      {
        return LimitExecuteTime(() => GetAddRoute(req));
      }
      catch(Exception ex)
      {
        return WriteError(req, ex.Message);
      }
    }

    private HttpResponse GetAddRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<AddRequest>();
        if (er?.Documents == null || string.IsNullOrEmpty(er.Language))
          return WriteError(req, Resources.WebErrorInvalidPostData);
        if (er.Documents.Length > 100 || er.Documents.Sum(x=>x.Text.Length) / 5000 < 100)
          return WriteError(req, Resources.WebErrorPostMax100Pages);

        return UseTagger(req, er.Language, er.GetDocumentArray(), true);
      }
      catch (Exception ex)
      {
        return WriteError(req, ex.Message);
      }
    }

    private HttpResponse UseTagger(HttpRequest req, string language, Dictionary<string, object>[] docs,
                                   bool enableCleanup)
    {
      var tagger = new SimpleTreeTagger();
      var available = new HashSet<string>(tagger.LanguagesAvailabel);
      if (!available.Contains(language))
        return WriteError(req, string.Format(Resources.WebErrorWrongLanguage, string.Join(", ", available)));

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
        return WriteError(req, Resources.WebErrorTaggingProcessError);

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
          return WriteError(req, Resources.WebErrorInvalidPostData);

        var aCheck = Configuration.GetConsoleAction(er.Action);
        if (aCheck == null || !ExecuteActionFilter.Check(er.Action))
          return WriteError(req, Resources.WebErrorActionUnavailable);

        if (!File.Exists($"corpora/{er.CorpusId}.cec6"))
          return WriteError(req, Resources.WebErrorCorpusUnavailable);

        var corpus = CorpusAdapterWriteDirect.Create($"corpora/{er.CorpusId}.cec6");
        if (corpus == null)
          return WriteError(req, Resources.WebErrorCorpusUnavailable);

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
        return WriteError(req, ex.Message);
      }
    }

    protected override Server ConfigureServer(Server server)
    {
      server.AddEndpoint(HttpVerb.GET, "/add/languages/", AvailableLanguagesRoute);
      server.AddEndpoint(HttpVerb.POST, "/add/", AddRoute);

      if (!Directory.Exists("corpora"))
        Directory.CreateDirectory("corpora");

      return server;
    }

    protected override SericeDocumentation GetDocumentation()
    {
      return new SericeDocumentation
      {
        Description = "CorpusExplorer-Free100-Endpoint (Version 1.0.0)",
        Url = Url,
        Endpoints = new[]
        {
          new ServiceEndpoint
          {
            Url = $"{Url}add/langauges/",
            AllowedVerbs = new[] {"GET"},
            Arguments = null,
            Description = string.Format(Resources.WebHelpListAvailableLanguages, Url),
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "languages", Type = "string array", Description = Resources.WebHelpParameterLanguages}
            }
          },
          new ServiceEndpoint
          {
            Url = $"{Url}add/",
            AllowedVerbs = new[] {"POST"},
            Arguments = null,
            Description = Resources.WebHelpAddCorpus,
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "language", Type = "string", Description = Resources.WebHelpAddCorpusParameterLanguage},
              new ServiceParameter
              {
                Name = "documents", Type = "array of objects",
                Description = Resources.WebHelpAddCorpusParameterDocuments
              }
            }
          },
          new ServiceEndpoint
          {
            Url = $"{Url}execute/",
            AllowedVerbs = new[] {"POST"},
            Description = string.Format(Resources.WebHelpExecute, Url),
            Arguments = new[]
            {
              new ServiceArgument
              {
                Name = "corpusId", Type = "string", Description = string.Format(Resources.WebHelpExecuteParameterCorpusId, Url),
                IsRequired = true
              },
              new ServiceArgument
                {Name = "action", Type = "string", Description = Resources.WebHelpExecuteParameterAction, IsRequired = true},
              new ServiceArgument
              {
                Name = "arguments", Type = "string-array", Description = Resources.WebHelpExecuteParameterArguments, IsRequired = true
              }
            },            
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "table", Type = "table (rows > array of objects)", Description = Resources.WebHelpExecuteResult}
            }
          }
        }
      };
    }
  }
}