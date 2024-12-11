using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.WaitBehaviour;
using CorpusExplorer.Sdk.Utils.WaitBehaviour.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model;
using CorpusExplorer.Terminal.Console.Web.Model.Response;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Tfres;

namespace CorpusExplorer.Terminal.Console.Web.Abstract
{
  public abstract class AbstractWebService : IDisposable
  {
    private readonly string _ip;
    private readonly int _port;
    private readonly int _timeout;
    private string _availableExecuteActions;
    private string _documentation;
    protected Server _server;

    protected AbstractWebService(AbstractTableWriter writer, string ip, int port, int timeout = 0)
    {
      Writer = writer;
      _ip = ip;
      _port = port;
      _timeout = timeout;
      Writer = writer;
      Mime = writer.MimeType;
      Url = $"http://{ip}:{port}/";
      InitializeDefaultParameter();
    }

    /// <summary>
    /// Filter, der angibt, welche Acitons (nicht) verfügbar sind.
    /// </summary>
    protected abstract ActionFilter ExecuteActionFilter { get; }

    /// <summary>
    /// Exporter
    /// </summary>
    protected AbstractTableWriter Writer { get; }

    /// <summary>
    /// MIME-Type, den der Server bereistellt. Hängt vom Parameter F: ab.
    /// </summary>
    protected string Mime { get; }

    /// <summary>
    /// Url des Servers
    /// </summary>
    protected string Url { get; }

    private void InitializeDefaultParameter()
    {
      _availableExecuteActions = JsonConvert.SerializeObject(InitializeExecuteActionList());
    }

    private AvailableActionsResponse InitializeExecuteActionList()
    {
      return ActionFilterToResponse(ExecuteActionFilter);
    }

    private AvailableActionsResponse ActionFilterToResponse(ActionFilter actionFilter)
    {
      return new AvailableActionsResponse
      {
        Items = (from action in Configuration.AddonConsoleActions
                 where actionFilter.Check(action.Action)
                 select new AvailableActionsResponse.AvailableActionsResponseItem
                 {
                   action = action.Action,
                   description = action.Description
                 }).ToArray()
      };
    }

    /// <summary>
    /// Startet den Webserver
    /// </summary>
    public void Run(AbstractWaitBehaviour waitBehaviour = null, Action<Task> continueWith = null)
    {
      System.Console.Write($"SERVER {Url} ...");
      _documentation = AppendDefaultDocumentation(GetDocumentation()).ConvertToJson();

      _server = new Server(_ip, _port, OpenApiRoute, continueWith) { Timeout = _timeout };
      _server.AddEndpoint(HttpMethod.Get, "/execute/actions/", ExecuteActionsRoute);
      _server.AddEndpoint(HttpMethod.Post, "/execute/", ExecuteRoute);
      _server = ConfigureServer(_server);
      System.Console.WriteLine(_server != null ? "ready!" : "error!");

      if(waitBehaviour == null)
        waitBehaviour = new WaitBehaviourWindows();

      waitBehaviour.Wait();
    }

    private void OpenApiRoute(HttpContext req)
    {
      req.Response.Send(_documentation, Mime);
    }

    private void ExecuteActionsRoute(HttpContext arg)
    {
      try
      {
        arg.Response.Send(_availableExecuteActions);
      }
      catch (Exception ex)
      {
        WriteError(arg, ex.Message);
      }
    }

    private OpenApiDocument AppendDefaultDocumentation(OpenApiDocument document)
    {
      document.Info = new OpenApiInfo
      {
        License = new OpenApiLicense { Name = "GNU Affero General Public License 3.0", Url = new Uri("https://www.gnu.org/licenses/agpl-3.0.de.html") },
        Contact = new OpenApiContact { Name = "Jan Oliver Rüdiger", Url = new Uri("https://notes.jan-oliver-ruediger.de/kontakt/") },
        TermsOfService = new Uri("https://www.gnu.org/licenses/agpl-3.0.de.html"),
        Title = "CorpusExplorer REST-WebService",
        Version = "1.0.0"
      };

      document.Servers = new List<OpenApiServer> {
        new OpenApiServer{ Url = $"http://{_ip}:{_port}" }
      };

      document.Paths.Add("/execute/actions/",
       new OpenApiPathItem
       {
         Operations = new Dictionary<OperationType, OpenApiOperation>
         {
            {
              OperationType.Get, new OpenApiOperation
              {
                Description= $"Lists all available actions for {Url}execute/",
                Responses = new OpenApiResponses
                { 
                  {"200", new OpenApiResponse{ Description ="action = The name of the action / description = Short description - action and parameter" } }
                }
              }
            }
         }
       });

      return document;
    }

    /// <summary>
    /// Erzeugt eine Fehlermeldung
    /// </summary>
    /// <param name="request">Ursprüngliche Abfrage</param>
    /// <param name="message">Nachricht</param>
    /// <returns>Fehlermeldung</returns>
    protected void WriteError(HttpContext request, string message)
    {
      using (var ms = new MemoryStream())
      {
        var writer = Writer.Clone(ms);
        writer.WriteError(message);
        writer.Destroy(false);

        ms.Seek(0, SeekOrigin.Begin);
        request.Response.Send(Encoding.UTF8.GetString(ms.ToArray()).Replace("\r\n", ""), Mime);
      }
    }

    /// <summary>
    /// Hier sollte die ergänzende Dokumentation aufgeführt werden. Dokumentiert ist bereits /execute/actions/. /execute/ und weitere/zusätzliche Funktionen müssen hier aufgeführt werden.
    /// </summary>
    /// <returns>Zusätzliche Dokumentation</returns>
    protected abstract OpenApiDocument GetDocumentation();

    protected abstract Server ConfigureServer(Server server);

    /// <summary>
    /// Führt den Befehl aus. Timeout wird überwacht.
    /// </summary>
    /// <param name="req">Anfrage</param>
    /// <returns>Ergebnis</returns>
    protected abstract void GetExecuteRoute(HttpContext req);

    private void ExecuteRoute(HttpContext req)
    {
      try
      {
        GetExecuteRoute(req);
      }
      catch (Exception ex)
      {
        WriteError(req, ex.Message);
      }
    }

    public void Dispose()
    {
      _server?.Dispose();
    }
  }
}