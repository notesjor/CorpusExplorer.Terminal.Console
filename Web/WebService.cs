using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.WebOrbit.Model.Request;
using CorpusExplorer.Terminal.WebOrbit.Model.Response;
using Newtonsoft.Json;
using Tfres;
using HttpRequest = Tfres.HttpRequest;
using HttpResponse = Tfres.HttpResponse;

namespace CorpusExplorer.Terminal.Console.Web
{
  public static class WebService
  {
    private static string _availableActions;
    private static object _getAvailableActionsRouteLock = new object();
    private static AbstractTableWriter _writer;
    private static AbstractCorpusAdapter _corpus;
    private static string _mime;

    public static void Run(AbstractTableWriter writer, int port, AbstractCorpusAdapter corpus)
    {
      _corpus = corpus;
      _writer = writer;
      _mime = writer.MimeType;

      var s = new Server("127.0.0.1", port, DefaultRoute);

      s.AddEndpoint(HttpVerb.GET, "/actions/", GetAvailableActionsRoute);
      s.AddEndpoint(HttpVerb.POST, "/execute/", GetExecuteRoute);
    }

    private static HttpResponse GetExecuteRoute(HttpRequest req)
    {
      try
      {
        var er = req.PostData<ExecuteRequest>();
        if (er == null)
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, "no valid post-data"));

        var a = Configuration.GetConsoleAction(er.Action);
        if (a == null)
          return null;

        var selection = _corpus.ToSelection();
        if (er.DocumentGuids != null && er.DocumentGuids.Length > 0)
          selection = selection.CreateTemporary(er.DocumentGuids);

        using (var ms = new MemoryStream())
        {
          var writer = _writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          return new HttpResponse(req, true, 200, null, _mime, Encoding.UTF8.GetString(ms.ToArray()));
        }
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, ex.Message));
      }
    }

    private static HttpResponse GetAvailableActionsRoute(HttpRequest req)
    {
      lock (_getAvailableActionsRouteLock)
        try
        {
          if (_availableActions != null)
            return new HttpResponse(req, true, 200, null, _mime, _availableActions);

          var res = new AvailableActionsResponse
          {
            Items = Configuration.AddonConsoleActions.Select(action =>
                                                               new AvailableActionsResponse.
                                                                 AvailableActionsResponseItem
                                                               {
                                                                 action = action.Action,
                                                                 describtion = action.Description
                                                               }).ToArray()
          };
          _availableActions = JsonConvert.SerializeObject(res);

          return new HttpResponse(req, true, 200, null, _mime, _availableActions);
        }
        catch (Exception ex)
        {
          return new HttpResponse(req, false, 500, null, _mime, WriteError(_writer, ex.Message));
        }
    }

    private static string WriteError(AbstractTableWriter prototype, string message)
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

    private static HttpResponse DefaultRoute(HttpRequest req)
    {
      return new HttpResponse(req, true, 200, null, "text/plain", "CorpusExplorer-Endpoint (Version 1.0.0)");
    }
  }
}
