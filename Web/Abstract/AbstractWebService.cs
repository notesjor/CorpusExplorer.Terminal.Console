using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Web.Model;
using CorpusExplorer.Terminal.Console.Web.Model.Response;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Tfres;

namespace CorpusExplorer.Terminal.Console.Web.Abstract
{
  public abstract class AbstractWebService
  {
    private readonly string _ip;
    private readonly int _port;
    private readonly int _timeout;
    private string _availableExecuteActions;
    private string _documentation;

    protected AbstractWebService(AbstractTableWriter writer, string ip, int port, int timeout)
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

    /// <summary>
    /// Begrenzt die Ausführungszeit einer Funktion
    /// </summary>
    /// <param name="func">Funktion, deren Ausführungszeit begrenzt werden soll.</param>
    /// <returns>Rückgabewert der Funktion</returns>
    protected HttpResponse LimitExecuteTime(Func<HttpResponse> func)
    {
      var task = Task.Run(func);
      if (_timeout == 0)
        task.Wait();
      else
        task.Wait(_timeout);

      return task.Result;
    }

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
    public void Run()
    {
      System.Console.Write($"SERVER {Url} ...");
      _documentation = OpenApiHelper.ConvertToJson(AppendDefaultDocumentation(GetDocumentation()));

      var s = new Server(_ip, _port, DefaultRoute);
      s.AddEndpoint(HttpVerb.GET, "/execute/actions/", ExecuteActionsRoute);
      s.AddEndpoint(HttpVerb.POST, "/execute/", ExecuteRoute);
      s = ConfigureServer(s);
      System.Console.WriteLine(s != null ? "ready!" : "error!");

      while (true) System.Console.ReadLine();
    }

    private HttpResponse DefaultRoute(HttpRequest req)
    {
      return new HttpResponse(req, true, 200, null, "application/json", _documentation);
    }

    private HttpResponse ExecuteActionsRoute(HttpRequest arg)
    {
      try
      {
        return LimitExecuteTime(() => new HttpResponse(arg, true, 200, null, "application/json", _availableExecuteActions));
      }
      catch (Exception ex)
      {
        return WriteError(arg, ex.Message);
      }
    }

    private OpenApiDocument AppendDefaultDocumentation(OpenApiDocument document)
    {
      document.Info = new OpenApiInfo
      {
        License = new OpenApiLicense { Name = "GNU Affero General Public License 3.0", Url = new Uri("https://www.gnu.org/licenses/agpl-3.0.de.html") },
        Contact = new OpenApiContact { Name = "Jan Oliver Rüdiger", Url = new Uri("https://notes.jan-oliver-ruediger.de/kontakt/") },
        TermsOfService = new Uri("https://www.gnu.org/licenses/agpl-3.0.de.html"),
        Title = "CorpusExplorer REST-WebServide",
        Version = "1.0.0"
      };

      document.Servers = new List<OpenApiServer> {
        new OpenApiServer{ Url = $"http://{_ip}:{_port}" }
      };

      document.Paths.Add($"{Url}execute/actions/",
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
                  {"action", new OpenApiResponse{ Description ="The name of the action" } },
                  {"description", new OpenApiResponse{ Description = "Short description - action and parameter" } }
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
    protected HttpResponse WriteError(HttpRequest request, string message)
    {
      using (var ms = new MemoryStream())
      {
        var writer = Writer.Clone(ms);
        writer.WriteError(message);
        writer.Destroy(false);

        ms.Seek(0, SeekOrigin.Begin);
        return new HttpResponse(request, false, 500, null, Mime, Encoding.UTF8.GetString(ms.ToArray()).Replace("\r\n", ""));
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
    protected abstract HttpResponse GetExecuteRoute(HttpRequest req);

    private HttpResponse ExecuteRoute(HttpRequest req)
    {
      try
      {
        return LimitExecuteTime(() => GetExecuteRoute(req));
      }
      catch (Exception ex)
      {
        return WriteError(req, ex.Message);
      }
    }
  }
}