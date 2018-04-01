using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Model;

namespace CorpusExplorer.Terminal.Console.Action.Abstract
{
  public abstract class AbstractAction
  {
    public abstract string Action { get; }

    public abstract string Description { get; }

    public abstract void Execute(Selection selection, string[] args);

    protected void WriteTable(DataTable table)
    {
      ConsoleConfiguration.Writer.WriteTable(table);
    }
  }
}