using System;
using System.IO;
using System.Text;
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
using System.Threading.Tasks;

namespace CorpusExplorer.Terminal.Console.Web
{
  public class WebService : AbstractWebService
  {
    private readonly AbstractCorpusAdapter _corpus;

    public WebService(AbstractTableWriter writer, string ip, int port, string file, int timeout) : base(writer, ip, port, timeout)
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

    protected override Task GetExecuteRoute(HttpContext req)
    {
      try
      {
        var er = req.PostData<ExecuteRequest>();
        if (er == null)
          return WriteError(req, Resources.WebErrorInvalidPostData);

        var a = Configuration.GetConsoleAction(er.Action);
        if (a == null || !ExecuteActionFilter.Check(er.Action))
          return WriteError(req, Resources.WebErrorActionUnavailable);
        if (er.Action == "cluster" && !ExecuteActionFilter.Check(er.Arguments[1]))
          return WriteError(req, Resources.WebErrorActionUnavailable);

        var selection = _corpus.ToSelection();
        if (er.DocumentGuids != null && er.DocumentGuids.Length > 0)
          selection = selection.CreateTemporary(er.DocumentGuids);

        using (var ms = new MemoryStream())
        {
          var writer = Writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          req.Response.ContentType = Mime;
          return req.Response.Send(ms.Length, ms);
        }
      }
      catch (Exception ex)
      {
        return WriteError(req, ex.Message);
      }
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
                      new OpenApiParameter{ Name = "guids", Required = false, Description = Resources.WebHelpExecuteParameterGuids},
                    },
                    Responses = new OpenApiResponses
                    {
                      { "200", new OpenApiResponse{ Description = Resources.WebHelpExecuteResult } }
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