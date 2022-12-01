using CorpusExplorer.Sdk.Addon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class PhraseCountAction : IAction
  {
    public string Action => "kwic-phrase-count";
    public string Description => Resources.DescKwicPhraseCount;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);

      queries = FileQueriesHelper.ResolveFileQueries(queries);

      var spanHelper = new KwicSpanHelper(queries);

      var dt = new DataTable();
      dt.Columns.Add("Phrase", typeof(string));
      dt.Columns.Add("Frequenz", typeof(int));

      dt.BeginLoadData();
      foreach (var q in GetQuery(args[0], spanHelper.CleanArguments))
      {
        var vm = new TextLiveSearchViewModel
        {
          Selection = selection,
          AddContextSentencesPre = spanHelper.SentencePre,
          AddContextSentencesPost = spanHelper.SentencePost,
        };

        vm.AddQuery(q);
        vm.Execute();

        dt.Rows.Add(string.Join(" ", q.LayerQueries), vm.ResultCountWords);
      }
      dt.EndLoadData();

      writer.WriteTable(selection.Displayname, dt);
    }

    protected IEnumerable<FilterQuerySingleLayerExactPhrase> GetQuery(string layerDisplayname, IEnumerable<string> queries)
      => queries.Select(q => new FilterQuerySingleLayerExactPhrase
      {
        LayerDisplayname = layerDisplayname,
        LayerQueries = q.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries)
      });
  }
}
