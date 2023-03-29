using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using ConsoleTableExt;
using Microsoft.AspNetCore.Identity;

namespace Biobanks.Directory.Commands.Runners;

public class ManageUserRoles
{
  private readonly IConsole _console;
  private readonly UserManager<ApplicationUser> _users;

  public ManageUserRoles(
    IConsole console,
    UserManager<ApplicationUser> users)
  {
    _console = console;
    _users = users;
  }

  public async Task Run(string email, List<string> removeRoles, List<string> addRoles)
  {
    var user = await _users.FindByEmailAsync(email);

    var outputRows = new List<List<object>>
    {
      new() { "User Email", email },
    };

    if (removeRoles.Any())
    {
      await _users.RemoveFromRolesAsync(user, removeRoles);
      outputRows.Add(new() { "Roles Removed", string.Join(", ", removeRoles) });
    }

    if (addRoles.Any())
    {
      await _users.AddToRolesAsync(user, addRoles);
      outputRows.Add(new() { "Roles Added", string.Join(", ", addRoles) });
    }

    var currentRoles = await _users.GetRolesAsync(user);
    outputRows.Add(new() { "Current Roles", string.Join(", ", currentRoles) });

    _console.Out.Write(ConsoleTableBuilder
      .From(outputRows)
      .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
      .Export()
      .ToString());
  }
}
