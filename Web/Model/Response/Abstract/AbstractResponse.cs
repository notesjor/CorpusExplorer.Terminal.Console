using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Terminal.WebOrbit.Model.Response.Abstract
{
  public abstract class AbstractResponse
  {
    public string error { get; set; }
    public bool hasError => !string.IsNullOrEmpty(error);
  }
}
