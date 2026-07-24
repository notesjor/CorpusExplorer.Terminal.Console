using CorpusExplorer.Sdk.Action.Abstract;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Adapter.Corpus.Abstract;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaImportAction : AbstractActionWithExport
  {
    public override string Action => "meta-import";
    public override string Description => Resources.DescMetaImport;

    protected override AbstractCorpusAdapter ExecuteCall(Selection selection, string[] args, string path)
    {
      if (args.Length != 1)
        return null;
      var vm = new DocumentMetadataViewModel { Selection = selection };
      vm.Execute();
      vm.Import(args[0]);
      vm.Execute();

      return selection.ToCorpus();
    }
  }
}