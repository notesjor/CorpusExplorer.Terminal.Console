using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;

namespace CorpusExplorer.Sdk.Action
{
  public class ConvertAction : AbstractActionWithExport
  {
    public override string Action => "convert";
    public override string Description => Resources.DescConvert;
    protected override AbstractCorpusAdapter ExecuteCall(Selection selection, string[] args, string path)
      => selection.ToCorpus();
  }
}