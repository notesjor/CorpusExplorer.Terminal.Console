using System;
using System.Collections.Generic;
using CorpusExplorer.Sdk.Model.Adapter.Corpus;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class CorpusNullHelper
  {
    private static CorpusAdapterWriteDirect _corpus = null;

    public static CorpusAdapterWriteDirect Corpus
    {
      get
      {
        if (_corpus != null)
          return _corpus;

        _corpus = CorpusAdapterWriteDirect.Create(new Dictionary<Guid, Dictionary<string, object>>(),
                                                  new Dictionary<string, object>(), null);
        return _corpus;
      }
    }
  }
}
