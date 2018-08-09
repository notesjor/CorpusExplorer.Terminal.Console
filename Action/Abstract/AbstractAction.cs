using System.Collections.Generic;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;

namespace CorpusExplorer.Terminal.Console.Action.Abstract
{
  public abstract class AbstractAction : IAddonConsoleAction // Das Interface erlaub ein Einbinden als Add-on
  {
    public abstract string Action { get; }

    public abstract string Description { get; }

    public abstract void Execute(Selection selections, string[] args, AbstractTableWriter writer);
  }
}