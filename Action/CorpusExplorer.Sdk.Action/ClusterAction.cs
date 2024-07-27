using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
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

    public class InternalClusterTableWriter : AbstractTableWriter
    {
      private DataTable _dataTable;
      private AbstractTableWriter _writer;

      public InternalClusterTableWriter(AbstractTableWriter writer)
      {
        _writer = writer;
      }

      public override string TableWriterTag => "";

      public override string Description => "only internal usage";

      public override string MimeType => "";

      public override AbstractTableWriter Clone(Stream stream)
      {
        return new InternalClusterTableWriter(_writer.Clone(stream));
      }

      protected override void WriteBody(DataTable table)
      {
        foreach (DataRow row in table.Rows)
          _dataTable.Rows.Add(row.ItemArray);
      }

      protected override void WriteFooter()
      {
        _dataTable.EndLoadData();
        _writer.WriteTable(_dataTable);
      }

      protected override void WriteHead(DataTable table)
      {
        _dataTable = new DataTable();
        foreach (DataColumn column in table.Columns)
          _dataTable.Columns.Add(column.ColumnName, column.DataType);
        _dataTable.BeginLoadData();
      }
    }

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
      var clusterWriter = new InternalClusterTableWriter(writer);
      foreach (var s in selections) action.Execute(s, nargs.ToArray(), clusterWriter);
      clusterWriter.Destroy();
    }

    public void ExecuteXmlScriptProcessorBypass(AbstractActionWithExport action, Selection selection, string[] args, AbstractExporter exporter, string path)
    {
      if (args.Length < 2)
        return;

      try
      {
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
          var output = path.Replace("{cluster}", string.IsNullOrWhiteSpace(s.Displayname) ? "NONE" : s.Displayname);
          var directory = Path.GetDirectoryName(output);
          if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

          if (File.Exists(output) && new FileInfo(output).Length > 0)
            continue;

          action.ExecuteXmlScriptProcessorBypass(s, nargs.ToArray(), exporter, output);
        }
      }
      catch (Exception ex)
      {
        // ignore
      }
    }

    public void ExecuteXmlScriptProcessorBypass(IAction action, Selection selection, string[] args, AbstractTableWriter writer, string path, bool merge)
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

      if (merge)
      {
        if (File.Exists(path) && new FileInfo(path).Length > 0)
          return;

        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        using (var bs = new BufferedStream(fs))
        {
          var format = writer.Clone(bs);

          foreach (var s in selections)
            action.Execute(s, nargs.ToArray(), format);

          format.Destroy();
        }
      }
      else
        foreach (var s in selections)
        {
          writer.Path = path.Replace("{cluster}", string.IsNullOrWhiteSpace(s.Displayname) ? "NONE" : s.Displayname);

          var directory = Path.GetDirectoryName(writer.Path);
          if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

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