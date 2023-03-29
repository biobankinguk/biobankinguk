using System.Collections.Generic;
using System.CommandLine;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Commands.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands;

internal class AddUser : Command
{
  public AddUser(string name)
    : base(name, "Add a new User")
  {
    var argEmail = new Argument<string>("email", "The new user's email address");
    Add(argEmail);

    var argFullName = new Argument<string>("full-name", "The new user's Full Name");
    Add(argFullName);

    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

    var optPassword = new Option<string>(new[] { "--password", "-p" },
      "The password to set for the new user. If omitted, the password reset journey can be used to set a password.");
    Add(optPassword);

    var optRoles = new Option<List<string>>(new[] { "--roles", "-r" },
      "Role names to add the new user to. `users list-roles` can be used to list valid role names.")
    {
      AllowMultipleArgumentsPerToken = true
    };
    Add(optRoles);

    this.SetHandler(
      async (
        logger, config, console,
        email, fullName,
        overrideConnectionString,
        password, roles) =>
      {
        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? ConfigurationExtensions.GetConnectionString(config, "Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s =>
          {
            ServiceCollectionServiceExtensions.AddSingleton<ILoggerFactory>(s, _ => logger)
              .AddSingleton<IConfiguration>(_ => config)
              .AddSingleton<IConsole>(_ => console)
              .AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

            LoggingServiceCollectionExtensions.AddLogging(s)
              .AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            ServiceCollectionServiceExtensions.AddTransient<Runners.AddUser>(s);
          })
          .GetRequiredService<Runners.AddUser>()
          .Run(email, fullName, password, roles);
      },
      Bind.FromServiceProvider<ILoggerFactory>(),
      Bind.FromServiceProvider<IConfiguration>(),
      Bind.FromServiceProvider<IConsole>(),
      argEmail, argFullName,
      optConnectionString, optPassword, optRoles);
  }
}
