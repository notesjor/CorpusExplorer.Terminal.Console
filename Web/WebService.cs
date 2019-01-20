using System;
using System.IO;
using System.Text;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Terminal.Console.Helper;
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

    public WebService(AbstractTableWriter writer, int port, string file) : base(writer, port)
    {
      System.Console.WriteLine("INIT WebService (mode: file)");
      System.Console.Write($"LOAD: {file}...");
      _corpus = CorpusLoadHelper.LoadCorpus(file);
      System.Console.WriteLine("ok!");
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
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "no valid post-data"));

        var a = Configuration.GetConsoleAction(er.Action);
        if (a == null || er.Action == "convert" || er.Action == "query")
          return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, "action not available"));

        var selection = _corpus.ToSelection();
        if (er.DocumentGuids != null && er.DocumentGuids.Length > 0)
          selection = selection.CreateTemporary(er.DocumentGuids);

        using (var ms = new MemoryStream())
        {
          var writer = Writer.Clone(ms);
          a.Execute(selection, er.Arguments, writer);
          writer.Destroy(false);

          ms.Seek(0, SeekOrigin.Begin);
          return new HttpResponse(req, true, 200, null, Mime, Encoding.UTF8.GetString(ms.ToArray()));
        }
      }
      catch (Exception ex)
      {
        return new HttpResponse(req, false, 500, null, Mime, WriteError(Writer, ex.Message));
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
            Arguments = new[]
            {
              new ServiceArgument
                {Name = "action", Type = "string", Description = "name of the action to execute", IsRequired = true},
              new ServiceArgument
              {
                Name = "arguments", Type = "key-value",
                Description = "example: {'key1':'value1', 'key2':'value2', 'key3':'value3'}", IsRequired = true
              },
              new ServiceArgument
              {
                Name = "guids", Type = "array of guids (as string)",
                Description = "example: ['guid1', 'guid2', 'guid3']", IsRequired = false
              }
            },
            Description = $"Shows all available Actions for {Url}execute/",
            ReturnValue = new[]
            {
              new ServiceParameter
                {Name = "table", Type = "table (rows > array of objects)", Description = "execution result"}
            }
          }
        }
      };
    }
  }
}