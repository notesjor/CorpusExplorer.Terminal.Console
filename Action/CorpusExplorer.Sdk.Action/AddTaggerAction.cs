using System;
using System.IO;
using System.Linq;
using CorpusExplorer.Sdk.Action.Properties;
using CorpusExplorer.Sdk.Addon;
using CorpusExplorer.Sdk.Ecosystem.Model;
using CorpusExplorer.Sdk.Helper;
using CorpusExplorer.Sdk.Model;
using CorpusExplorer.Sdk.Model.Extension;
using CorpusExplorer.Sdk.Utils.DataTableWriter.Abstract;
using CorpusExplorer.Sdk.Utils.DocumentProcessing.Tagger.AdditionalTaggerWrapper;

namespace CorpusExplorer.Sdk.Action
{
  public class AddTaggerAction : IAction
  {
    public string Action => "add-tagger";
    public string Description => "add-tagger [TAGGER#LANGUAGE] [OUTPUT] - add another tagger to the corpus.";

    public void Execute(Selection selection, string[] args, AbstractTableWriter writer)
    {
      if (args.Length != 2)
        return;

      var tag = args[0].Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries);
      if (tag.Length != 2)
        return;

      var tagger = Configuration.AddonTaggers.GetReflectedType(tag[0], "Tagger");
      if (tagger == null)
        return;
      tagger.LanguageSelected = tag[1];

      var output = args[1].Split(Splitter.Hashtag, StringSplitOptions.RemoveEmptyEntries);
      if (output.Length != 2)
        return;

      var exporter = Configuration.AddonExporters.GetReflectedType(output[0], "Exporter");
      if (exporter == null)
        return;

      var wrapper = new AdditionalTaggerWrapper(tagger);
      wrapper.Input.Enqueue(selection.ToCorpus());
      wrapper.Execute();

      var path = output[1].Replace("\"", "");
      var dir = Path.GetDirectoryName(path);
      if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      exporter.Export(wrapper.Output.First(), path);
    }
  }
}