using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action.Abstract
{
  public abstract class AbstractAction
  {
    public abstract string Action { get; }

    public abstract string Description { get; }

    public abstract void Execute(Selection selection, string[] args, AbstractTableWriter writer);
  }
}