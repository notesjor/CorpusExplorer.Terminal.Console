using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorpusExplorer.Terminal.Console.Helper
{
  public static class ConsoleHelper
  {
    public static void PrintHeader()
    {
      System.Console.Clear();
      System.Console.WriteLine();
      System.Console.WriteLine("CorpusExplorer v2.0");
      System.Console.WriteLine($"Copyright 2013-{DateTime.Now.Year} by Jan Oliver Rüdiger");
      System.Console.WriteLine();
    }
  }
}
