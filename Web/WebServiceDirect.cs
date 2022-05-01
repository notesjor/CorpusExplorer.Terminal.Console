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

    public WebServiceDirect(AbstractTableWriter writer, string ip, int port, int timeout = 0) : base(writer, ip, port, timeout)
    {
      _availableLanguages = JsonConvert.SerializeObject(InitializeAvailableLanguagesList());
    }

    private string[] InitializeAvailableLanguagesList()
    {
      return new SimpleTreeTagger().LanguagesAvailabel.ToArray();
    }

    private void AvailableLanguagesRoute(HttpContext arg)
    {
      try
      {
        arg.Response.Send(_availableLanguages, "application/json");
      }
      catch (Exception ex)
      {
        WriteError(arg, ex.Message);
      }
    }

    protected override ActionFilter ExecuteActionFilter
      => new ActionFilter(false, "convert", "query", "cluster");

    private void AddRoute(HttpContext req)
    {
      try
      {
        GetAddRoute(req);
      }
      catch (Exception ex)
      {
        WriteError(req, ex.Message);
      }
    }

    private void GetAddRoute(HttpContext req)
    {
      try
      {
        var er = req.PostData<AddRequest>();
        if (er?.Documents == null || string.IsNullOrEmpty(er.Language))
        {
          WriteError(req, Resources.WebErrorInvalidPostData);
          return;
        }
        if (er.Documents.Length > 100 || er.Documents.Sum(x => x.Text.Length) / 5000 < 100)
        {
          WriteError(req, Resources.WebErrorPostMax100Pages);
          return;
        }

        UseTagger(req, er.Language, er.GetDocumentArray(), true);
      }
      catch (Exception ex)
      {
        WriteError(req, ex.Message);
      }
    }

    private void UseTagger(HttpContext req, string language, Dictionary<string, object>[] docs,
                                   bool enableCleanup)
    {
      var tagger = new SimpleTreeTagger();
      var available = new HashSet<string>(tagger.LanguagesAvailabel);
      if (!available.Contains(language))
      {
        WriteError(req, string.Format(Resources.WebErrorWrongLanguage, string.Join(", ", available)));
        return;
      }

      if (enableCleanup)
      {
        var cleaner1 = new StandardCleanup();
        cleaner1.Input.Enqueue(docs);
        cleaner1.Execute();
        var cleaner2 = new RegexXmlMarkupCleanup { Input = cleaner1.Output };
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
      {
        WriteError(req, Resources.WebErrorTaggingProcessError);
        return;
      }

      corpus.Save($"corpora/{corpus.CorporaGuids.First()}.cec6", false);
      req.Response.Send($"{{ \"corpusId\": \"{corpus.CorporaGuids.First()}\" }}", "application/json");
    }

    protected override void GetExecuteRoute(HttpContext req)
    {
      try
      {
        var er = req.PostData<ExecuteRequest>();
        if (er == null)
        {
          WriteError(req, Resources.WebErrorInvalidPostData);
          return;
        }

        var aCheck = Configuration.GetConsoleAction(er.Action);
        if (aCheck == null || !ExecuteActionFilter.Check(er.Action))
        {
          WriteError(req, Resources.WebErrorActionUnavailable);
          return;
        }

        if (!File.Exists($"corpora/{er.CorpusId}.cec6"))
        {
          WriteError(req, Resources.WebErrorCorpusUnavailable);
          return;
        }

        var corpus = CorpusAdapterWriteDirect.Create($"corpora/{er.CorpusId}.cec6");
        if (corpus == null)
        {
          WriteError(req, Resources.WebErrorCorpusUnavailable);
          return;
        }

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
        req.Response.Send(response, Mime);
      }
      catch (Exception ex)
      {
        WriteError(req, ex.Message);
      }
    }

    protected override Server ConfigureServer(Server server)
    {
      server.AddEndpoint(System.Net.Http.HttpMethod.Get, "/add/languages/", AvailableLanguagesRoute);
      server.AddEndpoint(System.Net.Http.HttpMethod.Post, "/add/", AddRoute);

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
            "/execute/", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Post, new OpenApiOperation
                  {
                    Description = "Executes a command against a corpus (defined by corpusId)",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "request", In = ParameterLocation.Query, Required = true,
                        Description =
                          "Executes a command against the complete corpus or a sub-corpus (defined by GUIDs)",
                        Schema = new OpenApiSchema
                        {
                          Type = "object",
                          Properties = new Dictionary<string, OpenApiSchema>
                          {
                            {"action", new OpenApiSchema {Type = "string"}},
                            {
                              "arguments",
                              new OpenApiSchema
                                {Type = "object", AdditionalProperties = new OpenApiSchema {Type = "string"}}
                            },
                            {"corpusId", new OpenApiSchema {Type = "string"}}
                          }
                        }
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = Resources.WebHelpExecuteResult}}
                    }
                  }
                }
              }
            }
          },
          {
            "/add/langauges/", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = string.Format(Resources.WebHelpListAvailableLanguages, Url),
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = Resources.WebHelpParameterLanguages}}
                    }
                  }
                }
              }
            }
          },
          {
            "/add/", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Post, new OpenApiOperation
                  {
                    Description = Resources.WebHelpAddCorpus,
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "request", In = ParameterLocation.Query, Required = true,
                        Description = Resources.WebHelpAddCorpus,
                        Schema = new OpenApiSchema
                        {
                          Type = "object",
                          Properties = new Dictionary<string, OpenApiSchema>
                          {
                            {
                              "language",
                              new OpenApiSchema
                                {Type = "string", Description = "see /add/language/ for all available languages."}
                            },
                            {
                              "documents", new OpenApiSchema
                              {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                  Type = "object", 
                                  Properties = new Dictionary<string, OpenApiSchema>
                                  {
                                    {"text", new OpenApiSchema {Type = "string"}},
                                    {
                                      "metadata",
                                      new OpenApiSchema
                                        {Type = "object", AdditionalProperties = new OpenApiSchema {Type = "string"}}
                                    },
                                  }
                                }
                              }
                            }
                          }
                        }
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "corpusId"}}
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