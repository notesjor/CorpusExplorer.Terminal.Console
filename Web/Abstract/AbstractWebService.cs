using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model.Response;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Tfres;
using Tfres.Documentation;

namespace CorpusExplorer.Terminal.Console.Web.Abstract
{
  public abstract class AbstractWebService
  {
    private string _availableExecuteActions;
    private string _availableExportActions;
    private string _availableExportFormats;
    private string _documentation;
    private readonly int _port;

    protected AbstractWebService(AbstractTableWriter writer, int port)
    {
      Writer = writer;
      _port = port;
      Writer = writer;
      Mime = writer.MimeType;
      Url = $"http://127.0.0.1:{port}/";
      InitializeDefaultParameter();
    }

    private void InitializeDefaultParameter()
    {
      _availableExecuteActions = JsonConvert.SerializeObject(InitializeExecuteActionList());
      _availableExportActions = JsonConvert.SerializeObject(InitializeExportActionList());
      _availableExportFormats = JsonConvert.SerializeObject(InitializeExportFormatsList());
    }

    private string[] InitializeExportFormatsList()
      => Configuration.AddonExporters.GetReflectedTypeNameDictionary().Keys.ToArray();

    protected abstract AvailableActionsResponse InitializeExportActionList();

    protected abstract AvailableActionsResponse InitializeExecuteActionList();

    protected AbstractTableWriter Writer { get; }

    protected string Mime { get; }

    protected string Url { get; }

    public void Run()
    {
      System.Console.Write($"SERVER {Url} ...");
      _documentation = JsonConvert.SerializeObject(AppendDefaultDocumentation(GetDocumentation()));

      var s = new Server("127.0.0.1", _port, DefaultRoute);
      s.AddEndpoint(HttpVerb.GET, "/execute/actions/", GetExecuteActionsRoute);
      s.AddEndpoint(HttpVerb.POST, "/execute/", GetExecuteRoute);
      s.AddEndpoint(HttpVerb.GET, "/export/formats/", GetExportFormatsRoute);
      s.AddEndpoint(HttpVerb.POST, "/export/", GetExecuteExportRoute);
      s.AddEndpoint(HttpVerb.GET, "/export/actions/", GetExportActionsRoute);
      s = ConfigureServer(s);
      System.Console.WriteLine(s != null ? "ready!" : "error!");

      while (true) System.Console.ReadLine();
    }

    private HttpResponse GetExecuteActionsRoute(HttpRequest arg)
      => new HttpResponse(arg, true, 200, null, "application/json", _availableExecuteActions);

    private HttpResponse GetExportActionsRoute(HttpRequest arg)
      => new HttpResponse(arg, true, 200, null, "application/json", _availableExecuteActions);

    private HttpResponse GetExportFormatsRoute(HttpRequest arg)
      => new HttpResponse(arg, true, 200, null, "application/json", _availableExportFormats);

    private SericeDocumentation AppendDefaultDocumentation(SericeDocumentation getDocumentation)
    {
      var endpoints = new List<ServiceEndpoint>(getDocumentation.Endpoints)
      {
        new ServiceEndpoint
        {
          Url = $"{Url}execute/actions/",
          AllowedVerbs = new[] {"GET"},
          Arguments = null,
          Description = $"Lists all available Actions for {Url}execute/",
          ReturnValue = new[]
          {
            new ServiceParameter
              {Name = "action", Type = "string", Description = "The name of the action"},
            new ServiceParameter
              {Name = "description", Type = "string", Description = "Short description - action and parameter"}
          }
        },
        new ServiceEndpoint
        {
          Url = $"{Url}export/actions/",
          AllowedVerbs = new[] {"GET"},
          Arguments = null,
          Description = $"Lists all available Actions for {Url}export/",
          ReturnValue = new[]
          {
            new ServiceParameter
              {Name = "action", Type = "string", Description = "The name of the action"},
            new ServiceParameter
              {Name = "description", Type = "string", Description = "Short description - action and parameter"}
          }
        },
        new ServiceEndpoint
        {
          Url = $"{Url}export/formats/",
          AllowedVerbs = new[] {"GET"},
          Arguments = null,
          Description = $"Lists all available export formats for {Url}export/",
          ReturnValue = new[]
          {
            new ServiceParameter
              {Name = "format", Type = "array of string", Description = "An array of valid export formats"}
          }
        }
      };
      getDocumentation.Endpoints = endpoints.OrderBy(x => x.Url).ToArray();
      return getDocumentation;
    }

    protected string WriteError(AbstractTableWriter prototype, string message)
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

    protected abstract SericeDocumentation GetDocumentation();

    protected abstract Server ConfigureServer(Server server);

    protected abstract HttpResponse GetExecuteRoute(HttpRequest req);

    private HttpResponse DefaultRoute(HttpRequest req)
      => new HttpResponse(req, true, 200, null, "application/json", _documentation);
  }
}