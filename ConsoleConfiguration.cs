using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Terminal.Console.Writer;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console
{
  public static class ConsoleConfiguration
  {
    public static AbstractTableWriter Writer { get; set; } = new CsvTableWriter();
  }
}
