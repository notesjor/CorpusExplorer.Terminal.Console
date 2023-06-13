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
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.Blocks;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Generator;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Generator.Abstract;
using System.Linq;
using CorpusExplorer.Sdk.Blocks.SelectionCluster.Cluster.Helper;

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
      server.AddEndpoint(HttpMethod.Get, "/fast/norm", FastNormData);
      server.AddEndpoint(HttpMethod.Get, "/fast/count", FastCount);
      server.AddEndpoint(HttpMethod.Get, "/fast/kwic", FastKwic);
      server.AddEndpoint(HttpMethod.Get, "/fast/timeline", FastTimeline);
      return server;
    }

    private void FastNormData(HttpContext context)
    {
      try
      {
        var getData = context.GetData();        
        var select = "all"; // select kann auch Y, YM oder YMD sein.
        if (getData.ContainsKey("selection"))
          select = getData["selection"];

        if (select == "all")
        {
          context.Response.Send(new
          {
            Corpora = _project.SelectAll.CountCorpora,
            Documents = _project.SelectAll.CountDocuments,
            Sentences = _project.SelectAll.CountSentences,
            Tokens = _project.SelectAll.CountToken
          });
        }
        else
        {
          var meta = "Datum";
          if (getData.ContainsKey("date-meta"))
            meta = getData["date-meta"];

          var block = _project.SelectAll.CreateBlock<SelectionClusterBlock>();
          block.ClusterGenerator = GetFastCluster(select); // select ist Y, YM oder YMD.
          block.ClusterGenerator.MetadataKey = meta;
          block.Calculate();

          var info = "simple";
          if (getData.ContainsKey("info"))
            info = getData["info"];

          if (info == "full")
          {
            var dict = new Dictionary<string, object>();
            foreach(var x in block.SelectionClusters)
            {
              var s = _project.SelectAll.CreateTemporary(x.Value);
              dict.Add(x.Key, new
              {
                Corpora = s.CountCorpora,
                Documents = s.CountDocuments,
                Sentences = s.CountSentences,
                Tokens = s.CountToken
              });
            }

            context.Response.Send(dict);
          }
          else
          {
            context.Response.Send(block.SelectionClusters.ToDictionary(x => x.Key, x => _project.SelectAll.CreateTemporary(x.Value).CountToken));
          }          
        }
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private void FastTimeline(HttpContext context)
    {
      try
      {
        var getData = context.GetData();
        AbstractFilterQuery query = GetFastQuery(getData);
        var date = "YM";
        if (getData.ContainsKey("date"))
          date = getData["date"];
        var meta = "Datum";
        if (getData.ContainsKey("date-meta"))
          meta = getData["date-meta"];

        var block = _project.SelectAll.CreateBlock<SelectionClusterBlock>();
        block.ClusterGenerator = GetFastCluster(date);
        block.ClusterGenerator.MetadataKey = meta;
        block.Calculate();

        var info = "simple";
        if (getData.ContainsKey("info"))
          info = getData["info"];

        if (info == "full")
        {
          var res = new Dictionary<string, object>();
          foreach (var x in block.SelectionClusters)
          {
            var s = _project.SelectAll.CreateTemporary(x.Value);
            var vm = GetFastTLS(query, s);
            res.Add(x.Key, new
            {
              Corpora = vm.ResultCountCorpora,
              Documents = vm.ResultCountDocuments,
              Sentences = vm.ResultCountSentences,
              Tokens = vm.ResultCountTokens
            });
          }
          context.Response.Send(res);
        }
        else
        {
          var res = new Dictionary<string, int>();
          foreach (var x in block.SelectionClusters)
          {
            var s = _project.SelectAll.CreateTemporary(x.Value);
            var vm = GetFastTLS(query, s);
            res.Add(x.Key, vm.ResultCountTokens);
          }
          context.Response.Send(res);
        }
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private AbstractSelectionClusterGenerator GetFastCluster(string date)
    {
      switch (date)
      {
        case "Y":
          return new SelectionClusterGeneratorDateTimeYearOnlyValue();
        default: // YM
          return new SelectionClusterGeneratorDateTimeYearMonthOnlyValue();
        case "YMD":
          return new SelectionClusterGeneratorDateTimeYearMonthDayOnlyValue();
      }
    }

    private void FastKwic(HttpContext context)
    {
      try
      {
        var vm = GetFastTLS(GetFastQuery(context.GetData()));

        using (var ms = new MemoryStream())
        {
          var tw = base.Writer.Clone(ms);
          tw.WriteTable(vm.GetDataTable());
          tw.Destroy(false);

          var text = System.Text.Encoding.UTF8.GetString(ms.ToArray());
          context.Response.Send(text);
        }
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private void FastCount(HttpContext context)
    {
      try
      {
        var vm = GetFastTLS(GetFastQuery(context.GetData()));

        context.Response.Send(new
        {
          Corpora = vm.ResultCountCorpora,
          Documents = vm.ResultCountDocuments,
          Sentences = vm.ResultCountSentences,
          Tokens = vm.ResultCountTokens
        });
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private static AbstractFilterQuery GetFastQuery(Dictionary<string, string> getData)
    {
      var q = getData["q"].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

      AbstractFilterQuery query = null;
      if (q.Length > 1)
      {
        query = new FilterQuerySingleLayerExactPhrase
        {
          LayerDisplayname = "Wort",
          LayerQueries = q
        };
      }
      else
      {
        query = new FilterQuerySingleLayerAnyMatch
        {
          LayerDisplayname = "Wort",
          LayerQueries = q
        };
      }

      return query;
    }

    private TextLiveSearchViewModel GetFastTLS(AbstractFilterQuery query, Selection select = null)
    {
      var vm = new TextLiveSearchViewModel { Selection = select == null ? _project.SelectAll : select };
      vm.AddQuery(query);
      vm.Execute();
      return vm;
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