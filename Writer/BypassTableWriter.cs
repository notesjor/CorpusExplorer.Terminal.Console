using System.Data;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Writer
{
  public class BypassTableWriter : AbstractTableWriter
  {
    public override string TableWriterTag => "F:BYPASS";
    public override void WriteTable(DataTable table)
    {
      Table = table;
    }

    public DataTable Table { get; set; }
  }
}