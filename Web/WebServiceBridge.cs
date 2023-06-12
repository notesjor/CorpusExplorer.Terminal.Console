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
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Model;
using System.Net.Http;
using System.Net;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;

namespace CorpusExplorer.Terminal.Console.Web
{
  public class WebServiceBridge : AbstractWebService
  {
    private readonly Project _project;

    public WebServiceBridge(AbstractTableWriter writer, string ip, int port, int timeout = 0) : base(writer, ip, port, timeout)
    {
      System.Console.Write(Resources.WebInitBridge);
      _project = CorpusExplorerEcosystem.InitializeMinimal();
      System.Console.WriteLine(Resources.Ok);
    }

    protected override ActionFilter ExecuteActionFilter
      => new ActionFilter(false, "convert", "query", "remove-meta", "remove-layer", "meta-export", "meta-import");

    protected override Server ConfigureServer(Server server)
    {
      server.AddEndpoint(HttpMethod.Get, "/load", LoadCorpus);
      return server;
    }

    private void LoadCorpus(HttpContext context)
    {
      try
      {
        var get = context.Request.GetData();
        var res = false;
        if (get.ContainsKey("url"))
          res = LoadCorpusUrl(get["url"]);
        if (get.ContainsKey("file"))
          res = LoadCorpusFile(get["file"]);

        context.Response.Send(res ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);
      }
      catch (Exception ex) when (ex is FileNotFoundException || ex is WebException)
      {
        context.Response.Send(HttpStatusCode.NotFound);
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private bool LoadCorpusUrl(string url)
    {
      var ending = url.ToLower().EndsWith(".cec6.gz")
        ? ".cec6.gz"
        : url.ToLower().EndsWith(".cec6.lz4")
          ? ".cec6.lz4"
          : ".cec6";

      var path = "TEMP" + ending;

      using (var wc = new WebClient())
      {
        wc.DownloadFile(url, path);
        LoadCorpusFile(path);
      }

      try
      {
        File.Delete(path);
      }
      catch
      {
        // ignore
      }

      return true;
    }

    private bool LoadCorpusFile(string file)
    {
      if (!File.Exists(file))
        throw new FileNotFoundException();

      _project.Add(CorpusAdapterWriteDirect.Create(file));
      return true;
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

        var selection = _project.SelectAll;
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
            "/load/", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                   OperationType.Get, new OpenApiOperation
                   {
                     Description = $"Loads a url/file corpus",
                     Parameters = new List<OpenApiParameter>
                     {
                       new OpenApiParameter
                       {
                         Name = "file", In = ParameterLocation.Query, Required = false,
                         Description = "Path to a local CEC6-file",
                       }
                     },
                     Responses = new OpenApiResponses
                     {
                       { "200", new OpenApiResponse { Description = "corpus loade"} },
                       { "404", new OpenApiResponse { Description = "corpus not found"} },
                       { "500", new OpenApiResponse { Description = "unknown error"} },
                     }
                   }
                }
              }
            }
          },
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
                        Description = "Executes a command against all corpora",
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