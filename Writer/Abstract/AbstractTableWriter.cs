using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;

namespace CorpusExplorer.Terminal.Console.Writer.Abstract
{
  public abstract class AbstractTableWriter : IDisposable
  {
    private Stream _output;

    public AbstractTableWriter()
    {
      _output = System.Console.OpenStandardOutput();
    }

    public abstract string TableWriterTag { get; }

    public abstract void WriteTable(DataTable table);

    protected void WriteOutput(string line)
    {
      var buffer = Configuration.Encoding.GetBytes(line.Replace("&#", "#"));
      _output.Write(buffer, 0, buffer.Length);
    }

    public void Dispose()
    {
      _output?.Dispose();
    }
  }
}
