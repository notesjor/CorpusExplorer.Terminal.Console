using System.Collections.Generic;
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

    protected void WriteOutput(string line)
    {
      var buffer = Configuration.Encoding.GetBytes(line.Replace("&#", "#"));
      System.Console.OpenStandardOutput().Write(buffer, 0, buffer.Length);
    }
  }
}