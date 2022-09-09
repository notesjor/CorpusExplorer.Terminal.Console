using System.Collections.Generic;
using System.IO;
using CorpusExplorer.Sdk.Ecosystem.Model;

namespace CorpusExplorer.Sdk.Action.Helper
{
  public static class FileQueriesHelper
  {
    public static List<string> ResolveFileQueries(List<string> queries)
    {
      var res = new List<string>();
      foreach (var query in queries)
      {
        if (query.StartsWith("FILE:"))
          res.AddRange(ResolveFileQueries(query));
        else
          res.Add(query);
      }

      return res;
    }

    public static List<string> ResolveFileQueries(string path)
    {
      var res = new List<string>();
      foreach (var query in File.ReadAllLines(path.Replace("FILE:", ""), Configuration.Encoding))
      {
        if (query.StartsWith("FILE:"))
          res.AddRange(ResolveFileQueries(query));
        else
          res.Add(query);
      }

      return res;
    }
  }
}
