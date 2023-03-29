using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using ConsoleTableExt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands.Runners;

public class AddUser
{
  private readonly ILogger<AddUser> _logger;
  private readonly IConsole _console;
  private readonly UserManager<ApplicationUser> _users;

  public AddUser(ILoggerFactory logger, IConsole console, UserManager<ApplicationUser> users)
  {
    _logger = logger.CreateLogger<AddUser>();
    _console = console;
    _users = users;
  }

  public async Task Run(string email, string fullName, string password, List<string> roles)
  {
    var user = new ApplicationUser()
    {
      UserName = email,
      Email = email,
      Name = fullName,
      EmailConfirmed = true
    };

    var result = string.IsNullOrWhiteSpace(password)
      ? await _users.CreateAsync(user)
      : await _users.CreateAsync(user, password);

    if (!result.Succeeded)
    {
      _logger.LogInformation("User creation failed with errors for {email}", email);

      var errorRows = new List<List<object>>();

      foreach (var e in result.Errors)
      {
        _logger.LogError(e.Description);
        errorRows.Add(new() { e.Description });
      }

      _console.Out.Write(ConsoleTableBuilder
        .From(errorRows)
        .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
        .Export()
        .ToString());

      return;
    }

    var outputRows = new List<List<object>>
    {
      new() { "Email", email },
      new() { "Full Name", fullName }
    };
    
    if (roles.Any())
    {
      await _users.AddToRolesAsync(user, roles);
      outputRows.Add(new() { "Roles", string.Join(", ", roles) });
    }

    _console.Out.WriteLine("Successfully added the new user!");

    _console.Out.Write(ConsoleTableBuilder
      .From(outputRows)
      .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
      .Export()
      .ToString());
  }
}
