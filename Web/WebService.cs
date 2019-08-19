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
using Tfres;
using Tfres.Documentation;

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

    protected override HttpResponse GetExecuteRoute(HttpRequest req)
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

        string response;
        using (var ms = new MemoryStream())
        {
          var writer = Writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          response = Encoding.UTF8.GetString(ms.ToArray());          
        }
        return new HttpResponse(req, true, 200, null, Mime, response);
      }
      catch (Exception ex)
      {
        return WriteError(req, ex.Message);
      }
    }

    protected override SericeDocumentation GetDocumentation()
    {
      return new SericeDocumentation
      {
        Description = "CorpusExplorer-Endpoint (Version 1.0.0)",
        Url = Url,
        Endpoints = new[]
        {
          new ServiceEndpoint
          {
            Url = $"{Url}execute/",
            AllowedVerbs = new[] {"POST"},
            Description = string.Format(Resources.WebHelpExecute, Url),
            Arguments = new[]
            {
              new ServiceArgument
                {Name = "action", Type = "string", Description = Resources.WebHelpExecuteParameterAction, IsRequired = true},
              new ServiceArgument
              {
                Name = "arguments", Type = "string-array",
                Description = Resources.WebHelpExecuteParameterArguments, IsRequired = true
              },
              new ServiceArgument
              {
                Name = "guids", Type = "array of guids (as string)",
                Description = Resources.WebHelpExecuteParameterGuids, IsRequired = false
              }
            },            
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "table", Type = "table (rows > array of objects)", Description = Resources.WebHelpExecuteResult}
            }
          }
        }
      };
    }
  }
}