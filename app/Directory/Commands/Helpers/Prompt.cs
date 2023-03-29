using System.CommandLine;
using System.CommandLine.IO;

namespace Biobanks.Directory.Commands.Helpers;

public class Prompt
{
  private readonly IConsole _console;

  public Prompt(IConsole console)
  {
    _console = console;
  }

  public bool YesNo(string promptText, bool defaultValue = false)
  {
    var optionsText = defaultValue
      ? "(Y/n)"
      : "(y/N)";

    bool? result = null;
    while (result is null)
    {
      _console.Out.WriteLine();
      
      _console.Out.WriteLine(
        $"{promptText} {optionsText}");

      _console.Out.Write("> ");
      var keyInfo = System.Console.ReadKey();

      result = keyInfo.Key switch
      {
        System.ConsoleKey.Y => true,
        System.ConsoleKey.N => false,
        System.ConsoleKey.Enter => defaultValue,
        _ => null
      };

      _console.Out.WriteLine();
    }

    return result.GetValueOrDefault();
  }
}
