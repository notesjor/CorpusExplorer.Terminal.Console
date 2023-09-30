using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Sdk.Action
{
  public class Frequency1RawSelectAction : IAction
  {
    public string Action => "frequency1-raw-select";

    public string Description => Resources.DescFrequ1RawSelect;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args == null || args.Length < 2)
        return;

      var action = new Frequency1SelectAction();
      action.Normalize = false;
      action.Execute(selection, args, writer);
    }
  }
}
