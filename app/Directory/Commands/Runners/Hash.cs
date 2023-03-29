using System.Collections.Generic;
using System.CommandLine;
using Biobanks.Submissions.Api.Utilities.IdentityModel;
using ConsoleTableExt;
using Microsoft.Extensions.Logging;

namespace Biobanks.Submissions.Api.Commands.Runners;

public class Hash
{
  private readonly IConsole _console;
  private readonly ILogger<Hash> _logger;

  public Hash(
    ILoggerFactory logger, IConsole console)
  {
    _console = console;
    _logger = logger.CreateLogger<Hash>();
  }

  public void Run(string input)
  {
    _logger.LogInformation(
      $"Hashing {{{nameof(input)}}} with SHA256 as {{hashFormat}}",
      input,
      OutputFormat.Base64Url);

    var outputRows = new List<List<object>>
    {
      new() { "Input", input },
      new() { "SHA256", input.Sha256() }
    };

    _console.Out.Write(ConsoleTableBuilder
      .From(outputRows)
      .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
      .Export()
      .ToString());
  }
}
