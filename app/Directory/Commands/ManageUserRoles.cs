using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Commands.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobanks.Submissions.Api.Commands;

internal class ManageUserRoles : Command
{
  public ManageUserRoles(string name)
    : base(name, "Manage roles for a User; listing, adding and/or removing.")
  {
    var argEmail = new Argument<string>("email", "The user's email address");
    Add(argEmail);

    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

    var optRemoveRoles = new Option<List<string>>(new[] { "--remove-roles", "--remove" },
      "Role names to remove the user from. `users list-roles` can be used to list valid role names.")
    {
      AllowMultipleArgumentsPerToken = true
    };
    Add(optRemoveRoles);

    var optAddRoles = new Option<List<string>>(new[] { "--add-roles", "--add" },
      "Role names to add the user to. `users list-roles` can be used to list valid role names.")
    {
      AllowMultipleArgumentsPerToken = true
    };
    Add(optAddRoles);

    this.SetHandler(
      async (config, console,
        email,
        overrideConnectionString,
        removeRoles, addRoles) =>
      {
        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? config.GetConnectionString("Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s =>
          {
            s.AddSingleton(_ => config)
              .AddSingleton(_ => console)
              .AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

            s.AddLogging()
              .AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            s.AddTransient<Runners.ManageUserRoles>();
          })
          .GetRequiredService<Runners.ManageUserRoles>()
          .Run(email, removeRoles, addRoles);
      },
      Bind.FromServiceProvider<IConfiguration>(),
      Bind.FromServiceProvider<IConsole>(),
      argEmail,
      optConnectionString, optRemoveRoles, optAddRoles);
  }
}
