using System.Collections.Generic;
using System.Data;
using System.Linq;
using CorpusExplorer.Port.RProgramming.Api.Action.Abstract;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Port.RProgramming.Api.Action
{
  public class ReadingEaseAction : AbstractAction
  {
    protected override HashSet<string> MatchActionLabels
      => new HashSet<string> {"rease", "readingease", "reading-ease", "r-ease"};

    public override void Execute(Selection selection, IEnumerable<string> args)
    {
      var vm = new ReadingEaseViewModel {Selection = selection};
      vm.Analyse();
      var table = vm.GetDataTable();

      WriteOutput(string.Join("\t", from DataColumn x in table.Columns select x.ColumnName) + "\r\n");
      foreach (DataRow x in table.Rows)
        WriteOutput(string.Join("\t", from y in x.ItemArray select x.ToString()) + "\r\n");
    }
  }
}