using CorpusExplorer.Sdk.Addon;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.CorpusManipulation;
using CorpusExplorer.Sdk.ViewModel;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;

namespace CorpusExplorer.Sdk.Action
{
  public  class KwicExportAction : IAction
  {
    public string Action => "kwic-export";
    public string Description => Resources.DescKwicExport;
    
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length != 3)
        return;

      var layer = args[0];
      var queries = FileQueriesHelper.ResolveFileQueries(args[1].Replace("FILE:", ""));
      var output = args[2].Split(new[] {"#"}, StringSplitOptions.RemoveEmptyEntries);

      if(output.Length != 2)
        return;

      var spanHelper = new KwicSpanHelper(queries);
      
      var merger = new CorpusMergerQueryBasedKwic
      {
        AddContextSentencesPre = spanHelper.SentencePre,
        AddContextSentencesPost = spanHelper.SentencePost,
        FilterQueries = new AbstractFilterQuery[]
        {
          new FilterQuerySingleLayerAnyMatch
          {
            LayerDisplayname = layer,
            LayerQueries = spanHelper.CleanArguments
          }
        }
      };
      merger.Input(selection.ToCorpus());

      var exporter = Configuration.AddonExporters.GetReflectedType(output[0], "Exporter");
      if (exporter == null)
        return;

      var path = output[1].Replace("\"", "");
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      exporter.Export(selection, path);
    }
  }
}
