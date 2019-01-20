using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.ViewModel;

namespace CorpusExplorer.Sdk.Action
{
  public class NamedEntityAction : IAction
  {
    public string Action => "ner";
    public string Description => "ner [NERFILE] - performs a named entity recorgnition";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 1)
        return;

      var vm = new NamedEntityDetectionViewModel
      {
        Selection = selection,
        Model = Blocks.NamedEntityRecognition.Model.Load(args[0])
      };

      vm.Execute();

      writer.WriteTable(selection.Displayname, vm.GetDataTable());
    }
  }
}