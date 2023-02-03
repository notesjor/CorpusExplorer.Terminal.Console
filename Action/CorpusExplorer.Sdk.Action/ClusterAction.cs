using System.Collections.Generic;
using System.IO;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Exporter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using Microsoft.SqlServer.Server;

namespace CorpusExplorer.Sdk.Action
{
  public class ClusterAction : IAction
  {
    public string Action => "cluster";

    public string Description => Resources.DescCluster;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length < 2 || args[1].ToLower() == "cluster")
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;

      var selections = UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature)query);
      if (selections == null)
        return;

      var nargs = new List<string>(args);
      nargs.RemoveAt(0);
      var task = nargs[0];
      var action = Configuration.GetConsoleAction(task);
      if (action == null)
        return;

      nargs.RemoveAt(0);
      foreach (var s in selections) action.Execute(s, nargs.ToArray(), writer);
    }

    public void ExecuteXmlScriptProcessorBypass(AbstractActionWithExport action, Selection selection, string[] args, AbstractExporter exporter, string path)
    {
      if (args.Length < 2)
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;

      var selections = UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature)query);
      if (selections == null)
        return;

      var nargs = new List<string>(args);
      nargs.RemoveAt(0);
      nargs.RemoveAt(0);

      foreach (var s in selections)
      {
        var output = path.Replace("{cluster}", s.Displayname);
        if (File.Exists(output) && new FileInfo(output).Length > 0)
          continue;

        action.ExecuteXmlScriptProcessorBypass(s, nargs.ToArray(), exporter, output); 
      }
    }

    public void ExecuteXmlScriptProcessorBypass(IAction action, Selection selection, string[] args, AbstractTableWriter writer, string path)
    {
      if (args.Length < 2)
        return;

      var query = QueryParser.Parse(args[0]);
      if (!(query is FilterQueryUnsupportedParserFeature))
        return;

      var selections = UnsupportedQueryParserFeatureHelper.Handle(selection, (FilterQueryUnsupportedParserFeature)query);
      if (selections == null)
        return;

      var nargs = new List<string>(args);
      nargs.RemoveAt(0);
      nargs.RemoveAt(0);

      foreach (var s in selections)
      {
        writer.Path = path.Replace("{cluster}", s.Displayname);

        if (File.Exists(writer.Path) && new FileInfo(writer.Path).Length > 0)
          continue;
        
        // Kopie des TableWriter, um eine parallele Verarbeitung zu ermöglichen.
        using (var fs = new FileStream(writer.Path, FileMode.Create, FileAccess.Write))
        using (var bs = new BufferedStream(fs))
        {
          var format = writer.Clone(bs);
          action.Execute(s, nargs.ToArray(), format);
          format.Destroy();
        }        
      }
    }
  }
}