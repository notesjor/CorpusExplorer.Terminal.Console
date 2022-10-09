using System.Collections.Generic;
using System.Linq;
using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Helper;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Abstract;
using CorpusExplorer.Sdk.Utils.Filter.Queries;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class KwicMdaFilterAction : IAction
  {
    public string Action => "kwic-mda";

    public string Description => Resources.DescKwicMda;
    
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 1)
        return;

      var vm = new TextLiveSearchViewModel
      {
        Selection = selection,
        AddContextSentencesPre = args.Length > 1 ? int.Parse(args[1]) : 0,
        AddContextSentencesPost = args.Length > 2 ? int.Parse(args[2]) : 0
      };

      var mda = new FilterQueryMultiLayerComplex();
      mda.Load(args.First());
      vm.AddQuery(mda);
      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetUniqueDataTableCsv());
    }
  }
}