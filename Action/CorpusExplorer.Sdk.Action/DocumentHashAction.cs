using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class DocumentHashAction : IAction
  {
    public string Action => "hash";

    public string Description => Resources.DescHash;
    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      var vm = new DocumentHashViewModel {Selection = selection};
      if (args.Length >= 1)
        vm.LayerDisplayname = args[0];
      if (args.Length == 2)
        switch (args[1].ToUpper())
        {
          case "MD5":
            vm.HashAlgorithm = DocumentHashViewModel.Algorithm.MD5;
            break;
          case "SHA1":
            vm.HashAlgorithm = DocumentHashViewModel.Algorithm.SHA1;
            break;
          case "SHA256":
            vm.HashAlgorithm = DocumentHashViewModel.Algorithm.SHA256;
            break;
          case "SHA512":
            vm.HashAlgorithm = DocumentHashViewModel.Algorithm.SHA512;
            break;
          default:
            vm.HashAlgorithm = DocumentHashViewModel.Algorithm.SHA512;
            break;
        }
        
      vm.Execute();

      writer.WriteTable(vm.GetDataTable());
    }
  }
}
