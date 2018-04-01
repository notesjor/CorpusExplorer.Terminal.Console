using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;

namespace CorpusExplorer.Terminal.Console.Writer.Abstract
{
  public abstract class AbstractTableWriter
  {
    public abstract void WriteTable(DataTable table);

    protected void WriteOutput(string line)
    {
      var buffer = Configuration.Encoding.GetBytes(line.Replace("&#", "#"));
      System.Console.OpenStandardOutput().Write(buffer, 0, buffer.Length);
    }
  }
}
