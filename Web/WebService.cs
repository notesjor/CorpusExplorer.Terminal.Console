using System;
using System.Collections.Concurrent;
using System.IO;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
using CorpusExplorer.Terminal.Console.Properties;
using CorpusExplorer.Terminal.Console.Web.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model;
using CorpusExplorer.Terminal.Console.Web.Model.Request.WebService;
using Microsoft.OpenApi.Models;
using Tfres;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CorpusExplorer.Terminal.Console.Web
{
  public class WebService : AbstractWebService
  {
    private readonly AbstractCorpusAdapter _corpus;

    public WebService(AbstractTableWriter writer, string ip, int port, string file, int timeout = 0) : base(writer, ip, port, timeout)
    {
      System.Console.Write(Resources.WebInit, file);
      _corpus = CorpusLoadHelper.LoadCorpus(file);
      System.Console.WriteLine(Resources.Ok);
    }

    protected override ActionFilter ExecuteActionFilter
      => new ActionFilter(false, "convert", "query");

    protected override Server ConfigureServer(Server server)
    {
      return server;
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

        var a = Configuration.GetConsoleAction(er.Action);
        if (a == null || !ExecuteActionFilter.Check(er.Action))
        {
          WriteError(req, Resources.WebErrorActionUnavailable);
          return;
        }

        if (er.Action == "cluster" && !ExecuteActionFilter.Check(er.Arguments[1]))
        {
          WriteError(req, Resources.WebErrorActionUnavailable);
          return;
        }

        var selection = _corpus.ToSelection();
        if (er.DocumentGuids != null && er.DocumentGuids.Length > 0)
          selection = selection.CreateTemporary(er.DocumentGuids);

        using (var ms = new MemoryStream())
        {
          var writer = Writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          req.Response.Send(ms, Mime);
        }
      }
      catch (Exception ex)
      {
        WriteError(req, ex.Message);
      }
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
                    Description = string.Format(Resources.WebHelpExecute, Url),
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "request", In = ParameterLocation.Query, Required = true,
                        Description = "Executes a command against the complete corpus or a sub-corpus (defined by GUIDs)",
                        Schema = new OpenApiSchema
                        {
                          Type = "object",
                          Properties = new Dictionary<string, OpenApiSchema>
                          {
                            {"action", new OpenApiSchema {Type = "string"}},
                            {"arguments", new OpenApiSchema {Type = "object", AdditionalProperties = new OpenApiSchema {Type = "string"}}},
                            {"guids", new OpenApiSchema {Type = "array", Items = new OpenApiSchema{Type ="string"}}}
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
          }
        }
      };
    }
  }
}