using System;
using System.CommandLine;

namespace Biobanks.Submissions.Api.Commands;

public class Hash : Command
{
  public Hash(string commandName)
    : base(commandName, "A sub-command")
  {
    this.SetHandler(() => Console.WriteLine("hello"));
  }
}
