using Biobanks.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.IO;
using System.Threading.Tasks;

namespace Biobanks.Directory.Commands.Runners;

public class SetPassword
{
  private readonly ILogger<SetPassword> _logger;
  private readonly IConsole _console;
  private readonly UserManager<ApplicationUser> _users;

  public SetPassword(ILoggerFactory logger, IConsole console, UserManager<ApplicationUser> users)
  {
     _logger = logger.CreateLogger<SetPassword>();
    _console = console;
    _users = users;
  }

  public async Task Run(string email, string password)
  {
    var user = await _users.FindByEmailAsync(email);

    if (user == null)
    {
      _logger.LogInformation("User not found with the email {email}", email);
      _console.Out.WriteLine($"User not found with the email {email}");
      return;
    }

    await _users.RemovePasswordAsync(user);

   var addResult = await _users.AddPasswordAsync(user, password);
    if(!addResult.Succeeded)
    {
      foreach (var e in addResult.Errors)
      {
        _console.Out.WriteLine($"Error: {e.Description}");
        _logger.LogInformation("Password set error: {e}", e.Description);
      }
    }
    else
    {
      _console.Out.WriteLine("Succesfully set new password for user {email}");
    }
  }
}
