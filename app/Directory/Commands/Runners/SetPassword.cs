using Biobanks.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Threading.Tasks;

namespace Biobanks.Directory.Commands.Runners;

public class SetPassword
{
  private readonly ILogger<SetPassword> _logger;
  private readonly IConsole _console;
  private readonly UserManager<ApplicationUser> _users;

  public SetPassword(ILogger<SetPassword> logger, IConsole console, UserManager<ApplicationUser> users)
  {
  //  _logger = logger.CreateLogger<SetPassword>();
    _console = console;
    _users = users;
  }

  public async Task Run(string email, string password)
  {

  }
}
