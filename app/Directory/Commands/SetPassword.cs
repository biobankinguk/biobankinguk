using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Commands.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace Biobanks.Directory.Commands;

internal class SetPassword : Command
{
  public SetPassword(string email)
    : base(email, "Set a users password")
  {
    var argEmail = new Argument<string>("email", "The user's email address");
    Add(argEmail);

    var argPassword = new Argument<string>("password", "The user's new password");
    Add(argPassword);

    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

this.SetHandler(
   async(config, console,
      email,
      password,
      overrideConnectionString) =>
      {
        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? ConfigurationExtensions.GetConnectionString(config, "Default");

        // Configure DI and run the command handler
        await this
                  .ConfigureServices(s =>
                  {
          ServiceCollectionServiceExtensions.AddSingleton<IConfiguration>(s, _ => config)
            .AddSingleton<IConsole>(_ => console)
            .AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

          LoggingServiceCollectionExtensions.AddLogging(s)
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

          ServiceCollectionServiceExtensions.AddTransient<Runners.SetPassword>(s);
        })
                  .GetRequiredService<Runners.SetPassword>()
                  .Run(email, password);
        },
              Bind.FromServiceProvider<IConfiguration>(),
              Bind.FromServiceProvider<IConsole>(),
              argEmail,
              argPassword,
              optConnectionString);
  }
}
