using System.Collections.Generic;
using System.IO;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action.Abstract
{
  public abstract class AbstractFilterAction : IAction
  {
    public abstract string Action { get; }
    public abstract string Description { get; }

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var queries = new List<string>(args);
      queries.RemoveAt(0);

      queries = FileQueriesHelper.ResolveFileQueries(queries);

      var spanHelper = new KwicSpanHelper(queries);

      var vm = new TextLiveSearchViewModel
      {
        Selection = selection,
        AddContextSentencesPre = spanHelper.SentencePre,
        AddContextSentencesPost = spanHelper.SentencePost
      };

      vm.AddQuery(GetQuery(args[0], spanHelper.CleanArguments));
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetUniqueDataTableCsv());
    }
    
    protected abstract AbstractFilterQuery GetQuery(string layerDisplayname, IEnumerable<string> queries);
  }
}