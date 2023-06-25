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
using CorpusExplorer.Sdk.Helper;

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
      server.AddEndpoint(HttpMethod.Get, "/fast/norm", FastNormData);
      server.AddEndpoint(HttpMethod.Get, "/fast/count", FastCount);
      server.AddEndpoint(HttpMethod.Get, "/fast/kwic", FastKwic);
      server.AddEndpoint(HttpMethod.Get, "/fast/fulltext", FastFulltext);
      server.AddEndpoint(HttpMethod.Get, "/fast/cooc", FastCooccurrences);
      server.AddEndpoint(HttpMethod.Get, "/fast/timeline", FastTimeline);
      return server;
    }

    private CeDictionaryMemoryFriendly<double> _coocChache = null;
    private void FastCooccurrences(HttpContext context)
    {
      try
      {
        var getData = context.GetData();
        var query = getData["q"];

        if (_coocChache == null)
        {
          var block = _project.SelectAll.CreateBlock<CooccurrenceBlock>();
          block.Calculate();

          _coocChache = block.CooccurrenceSignificance.CompleteDictionaryToFullDictionaryMemoryFriendly();
          
          block.CooccurrenceFrequency.Clear();
          block.CooccurrenceSignificance.Clear();
          block = null;
          GC.Collect();
        }

        if (_coocChache.ContainsKey(query))
        {
          context.Response.Send(_coocChache[query]);
          return;
        }
        else
          context.Response.Send(HttpStatusCode.NotFound);
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
    }

    private void FastFulltext(HttpContext context)
    {
      try
      {
        var getData = context.GetData();
        var guid = Guid.Parse(getData["guid"]);
        var sentence = -1;
        if (getData.ContainsKey("sentence"))
          sentence = int.Parse(getData["sentence"]);
        var from = sentence;
        var to = sentence;
        if (getData.ContainsKey("from"))
          from = int.Parse(getData["from"]);
        if (getData.ContainsKey("to"))
          to = int.Parse(getData["to"]);

        if (from == to && from == -1)
          context.Response.Send(_project.SelectAll.GetReadableMultilayerDocument(guid));
        else
          context.Response.Send(_project.SelectAll.GetReadableMultilayerDocument(guid, from, to));
      }
      catch
      {
        context.Response.Send(HttpStatusCode.InternalServerError);
      }
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
          block.MetadataKey = meta;
          block.Calculate();

          var info = "simple";
          if (getData.ContainsKey("info"))
            info = getData["info"];

          if (info == "full")
          {
            var dict = new Dictionary<string, object>();
            foreach (var x in block.SelectionClusters)
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

        var preSelection = _project.SelectAll.CreateTemporary(query);

        var block = preSelection.CreateBlock<SelectionClusterBlock>();
        block.ClusterGenerator = GetFastCluster(date);
        block.MetadataKey = meta;
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
        case "C":
          return new SelectionClusterGeneratorDateTimeCenturyOnlyValue();
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

    public void UnloadCorpora()
    {
      _project.Clear();
    }

    public bool LoadCorpus(AbstractCorpusAdapter corpus)
    {
      _project.Add(corpus);
      if (_coocChache != null)
      {
        _coocChache.Clear();
        _coocChache = null;
      }

      return true;
    }

    public bool LoadCorpus(string file)
    {
      if (!File.Exists(file))
        throw new FileNotFoundException();

      return LoadCorpus(CorpusAdapterWriteDirect.Create(file));
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
            "/fast/norm", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Returns norm-data",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "selection", In = ParameterLocation.Query, Required = false,
                        Description = "If empty - Return information about all loaded corpora/snapshot. If value is 'date' you get a clustred result by date (see date-meta).",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "date-meta", In = ParameterLocation.Query, Required = false,
                        Description = "default (not set): Datum - please specify a DateTime-Value.",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "info", In = ParameterLocation.Query, Required = false,
                        Description = "If selection = date - you can set info to simple and then you only get the token per day as a response",
                        Schema = new OpenApiSchema {Type = "string"}
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "JSON-Array/Object of norm data."}}
                    }
                  }
                }
              }
            }
          },
          {
            "/fast/count", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Count occurrences of q",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "q", In = ParameterLocation.Query, Required = true,
                        Description = "Please URL-Encode complex data like umlauts or spaces",
                        Schema = new OpenApiSchema {Type = "string"}
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "Example: {\"Corpora\":1,\"Documents\":1438,\"Sentences\":45997,\"Tokens\":64298} - Tokens are the exact matches"}}
                    }
                  }
                }
              }
            }
          },
          {
            "/fast/kwic", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Get KWICs of q",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "q", In = ParameterLocation.Query, Required = true,
                        Description = "Please URL-Encode complex data like umlauts or spaces",
                        Schema = new OpenApiSchema {Type = "string"}
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "Example: \"CorpusID\":\"9ef22a04-a6e7-42cf-ad9b-9d636b61bc5a\",\"DocumentID\":\"64d7d1ab-5343-4c02-9b15-1937e14f9aeb\",\"SentenceID\":1,\"Pre\":\"This is the sentence part before the\",\"Match\":\"exact match\",\"Post\":\"and this is the rest of the sentence.\""}}
                    }
                  }
                }
              }
            }
          },
          {
            "/fast/fulltext", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Returns fulltext of a document",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "guid", In = ParameterLocation.Query, Required = true,
                        Description = "As you can see in /fast/kwic - you get a DocumentID. Use this here as the parameter",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "sentence", In = ParameterLocation.Query, Required = false,
                        Description = "0-based id of the sentence - if this and from/to is not set, you get the complete document",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "from", In = ParameterLocation.Query, Required = false,
                        Description = "For a more fine grained result (as sentence)",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "to", In = ParameterLocation.Query, Required = false,
                        Description = "For a more fine grained result (as sentence)",
                        Schema = new OpenApiSchema {Type = "string"}
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "Document as JSON-Array"}}
                    }
                  }
                }
              }
            }
          },
          {
            "/fast/cooc", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Get cooccurrences of q",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "q", In = ParameterLocation.Query, Required = true,
                        Description = "Please URL-Encode complex data like umlauts or spaces. Note: The first requests generates the model, all following requests are fast.",
                        Schema = new OpenApiSchema {Type = "string"}
                      }
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "List of cooccurrences and significance values"}}
                    }
                  }
                }
              }
            }
          },
          {
            "/fast/timeline", new OpenApiPathItem
            {
              Operations = new Dictionary<OperationType, OpenApiOperation>
              {
                {
                  OperationType.Get, new OpenApiOperation
                  {
                    Description = "Returns a timeline of the query q",
                    Parameters = new List<OpenApiParameter>
                    {
                      new OpenApiParameter
                      {
                        Name = "q", In = ParameterLocation.Query, Required = true,
                        Description = "Please URL-Encode complex data like umlauts or spaces. Note: The first requests generates the model, all following requests are fast.",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "date", In = ParameterLocation.Query, Required = false,
                        Description = "You can use the values C, Y, YM and YMD. This referes to century (C), year (Y), month (YM) and day (YMD)",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                      new OpenApiParameter
                      {
                        Name = "date-meta", In = ParameterLocation.Query, Required = false,
                        Description = "If you prefer another meta-data field then 'Datum' please use this parameter",
                        Schema = new OpenApiSchema {Type = "string"}
                      },
                    },
                    Responses = new OpenApiResponses
                    {
                      {"200", new OpenApiResponse {Description = "Document as JSON-Array"}}
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