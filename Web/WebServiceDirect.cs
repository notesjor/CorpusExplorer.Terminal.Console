using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Tfres;

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

    private Task AvailableLanguagesRoute(HttpContext arg)
    {
      try
      {
        return arg.Response.Send("application/json", _availableLanguages);
      }
      catch (Exception ex)
      {
        return WriteError(arg, ex.Message);
      }
    }

    protected override ActionFilter ExecuteActionFilter
      => new ActionFilter(false, "convert", "query", "cluster");

    private Task AddRoute(HttpContext req)
    {
      try
      {
        return GetAddRoute(req);
      }
      catch(Exception ex)
      {
        return WriteError(req, ex.Message);
      }
    }

    private Task GetAddRoute(HttpContext req)
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

    private Task UseTagger(HttpContext req, string language, Dictionary<string, object>[] docs,
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
      return req.Response.Send("application/json", $"{{ \"corpusId\": \"{corpus.CorporaGuids.First()}\" }}");
    }

    protected override Task GetExecuteRoute(HttpContext req)
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

        string response;
        using (var ms = new MemoryStream())
        {
          var writer = Writer.Clone(ms);
          a.Execute(selection, args.ToArray(), writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          response = Encoding.UTF8.GetString(ms.ToArray());          
        }
        return req.Response.Send(Mime, response);
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

    protected override OpenApiDocument GetDocumentation()
    {
      return new OpenApiDocument
      {
        Paths = new OpenApiPaths
        {
          {
            $"{Url}execute/", new OpenApiPathItem{
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Post, new OpenApiOperation
                  {
                    Description = string.Format(Resources.WebHelpExecute, Url),
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter{ Name = "action", Required = true, Description = Resources.WebHelpExecuteParameterAction},
                      new OpenApiParameter{ Name = "arguments", Required = true, Description = Resources.WebHelpExecuteParameterArguments},
                      new OpenApiParameter{ Name = "corpusId", Required = true, Description = string.Format(Resources.WebHelpExecuteParameterCorpusId, Url)},
                    },
                    Responses = new OpenApiResponses
                    {
                      { "200", new OpenApiResponse{ Description = Resources.WebHelpExecuteResult } }
                    }
                  }
                }
              }
            }
          },
          {
            $"{Url}add/langauges/", new OpenApiPathItem{
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = string.Format(Resources.WebHelpListAvailableLanguages, Url),
                    Responses = new OpenApiResponses
                    {
                      { "200", new OpenApiResponse{ Description = Resources.WebHelpParameterLanguages } }
                    }
                  }
                }
              }
            } 
          },
          {
            $"{Url}add/", new OpenApiPathItem{
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Post, new OpenApiOperation
                  {
                    Description = Resources.WebHelpAddCorpus,
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter{ Name = "language", Required = true, Description = Resources.WebHelpAddCorpusParameterLanguage},
                      new OpenApiParameter{ Name = "documents", Required = true, Description = Resources.WebHelpAddCorpusParameterDocuments}
                    },
                    Responses = new OpenApiResponses
                    {
                      { "200", new OpenApiResponse{ Description = "corpusId" } }
                    }
                  }
                }
              }
            }
          }
        }
      };
    }
  }
}