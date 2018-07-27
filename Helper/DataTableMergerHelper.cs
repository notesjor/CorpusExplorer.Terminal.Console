using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Utils.DataTableWriter;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class DataTableMergerHelper
  {
    public static void MergeDataTables(string cluster, ref DataTable dt, ref BypassTableWriter bypass)
    {
      foreach (DataColumn column in bypass.Table.Columns)
        if (!dt.Columns.Contains(column.ColumnName))
          dt.Columns.Add(column.ColumnName, column.DataType);
      
      dt.BeginLoadData();

      foreach (DataRow row in bypass.Table.Rows)
      {
        var items = new List<object> {cluster};
        items.AddRange(row.ItemArray);
        dt.Rows.Add(items.ToArray());
      }

      dt.EndLoadData();
    }
  }
}
