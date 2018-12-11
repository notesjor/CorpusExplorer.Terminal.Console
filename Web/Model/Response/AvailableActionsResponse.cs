using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorpusExplorer.Terminal.WebOrbit.Model.Response.Abstract;

namespace CorpusExplorer.Terminal.WebOrbit.Model.Response
{
  public class AvailableActionsResponse : AbstractResponse
  {
    public class AvailableActionsResponseItem
    {
      public string action { get; set; }
      public string description { get; set; }
    }

    public AvailableActionsResponseItem[] Items{get;set;}
  }
}
