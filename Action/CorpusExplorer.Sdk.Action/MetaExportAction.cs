using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class MetaExportAction : IAction
  {
    public string Action => "meta-export";
    public string Description => Resources.DescMetaExport;

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 1)
        return;

      var vm = new DocumentMetadataViewModel { Selection = selection };
      vm.Execute();
      vm.Export(args[0]);
    }
  }
}