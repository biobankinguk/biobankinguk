using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using ConsoleTableExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands.Runners;

public class ListRoles
{
  private readonly ILogger<ListRoles> _logger;
  private readonly IConsole _console;
  private readonly RoleManager<IdentityRole> _roles;

  public ListRoles(ILoggerFactory logger, IConsole console, RoleManager<IdentityRole> roles)
  {
    _logger = logger.CreateLogger<ListRoles>();
    _console = console;
    _roles = roles;
  }

  public Task Run()
  {
    var roles = _roles.Roles.Select(x => x.Name);

    var outputRows = new List<List<object>>();

    if (roles.Any())
    {
      foreach (var r in roles)
        outputRows.Add(new() { r });
    }
    else
    {
      _console.Out.WriteLine("There are no roles!");
    }

    _console.Out.Write(ConsoleTableBuilder
      .From(outputRows)
      .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
      .Export()
      .ToString());

    return Task.CompletedTask;
  }
}
