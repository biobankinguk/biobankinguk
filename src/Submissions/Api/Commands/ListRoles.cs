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

internal class ListRoles : Command
{
  public ListRoles(string name)
    : base(name, "List the names of user roles in the system.")
  {
    
    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

    
    this.SetHandler(
      async (
        logger, config, console,
        overrideConnectionString) =>
      {
        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? config.GetConnectionString("Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s =>
          {
            s.AddSingleton(_ => logger)
              .AddSingleton(_ => config)
              .AddSingleton(_ => console)
              .AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

            s.AddLogging()
              .AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            s.AddTransient<Runners.ListRoles>();
          })
          .GetRequiredService<Runners.ListRoles>()
          .Run();
      },
      Bind.FromServiceProvider<ILoggerFactory>(),
      Bind.FromServiceProvider<IConfiguration>(),
      Bind.FromServiceProvider<IConsole>(),
      optConnectionString);
  }
}
