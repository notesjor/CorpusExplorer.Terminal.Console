using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CorpusExplorer.Sdk.Ecosystem.Model;

namespace CorpusExplorer.Terminal.Console.Web.Model
{
  public class WebJobManager
  {
    private string _path;
    private Dictionary<string, string> _paths = new Dictionary<string, string>();
    private Action<string> _action;

    public WebJobManager(string managerName, Action<string> action)
    {
      ThreadPool.SetMinThreads((Configuration.ParallelOptions.MaxDegreeOfParallelism / 4) + 1, 0);
      ThreadPool.SetMaxThreads(1, 0);

      _path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"jobs/{managerName}");
      _action = action;
      try
      {
        if (!Directory.Exists(_path))
          Directory.CreateDirectory(_path);
      }
      catch
      {
        // ignore
      }
    }

    public string CreateJobId()
    {
      var guid = Guid.NewGuid().ToString("N");
      var path = Path.Combine(_path, guid);
      Directory.CreateDirectory(Path.Combine(_path, path));
      _paths.Add(guid, path);
      return guid;
    }

    public string GetJobDirectory(string jobId) => _paths.ContainsKey(jobId) ? _paths[jobId] : null;

    public void EnqueueJob(string jobId)
    {
      var path = GetJobDirectory(jobId);
      if (string.IsNullOrEmpty(path))
        return;

      ThreadPool.QueueUserWorkItem((obj)=> _action(obj as string), path);
    }
  }
}
