using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Terminal.Console.Writer.Abstract;

namespace CorpusExplorer.Terminal.Console.Writer
{
  public class CsvTableWriter : AbstractTableWriter
  {
    public override void WriteTable(DataTable table)
    {
      WriteOutput(string.Join(";", from DataColumn x in table.Columns select $"\"{x.ColumnName.Replace("\"", "''")}\"") + "\r\n");
      foreach (DataRow x in table.Rows)
      {
        var r = new string[table.Columns.Count];
        for (var i = 0; i < table.Columns.Count; i++)
        {
          r[i] = table.Columns[i].DataType == typeof(string)
            ? $"\"{x[i].ToString().Replace("\"", "''")}\""
            : x[i].ToString();
        }

        WriteOutput(string.Join(";", r) + "\r\n");
      }
    }
  }
}
