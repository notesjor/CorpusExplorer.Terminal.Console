using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class CustomParallelConfigurationHelper
  {
    public static ParallelOptions UseCustomParallelConfiguration(string parallel)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(parallel))
          return Configuration.ParallelOptions;

        var maxDegreeOfParallelism = int.Parse(parallel);
        if (maxDegreeOfParallelism > 1)
          return new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism };
        if (maxDegreeOfParallelism <= 0)
          return Configuration.ParallelOptions;
      }
      catch
      {
        // ignore
      }

      return new ParallelOptions { MaxDegreeOfParallelism = 1 };
    }
  }
}
