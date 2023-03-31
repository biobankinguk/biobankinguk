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

            ServiceCollectionServiceExtensions.AddTransient<Runners.ListRoles>(s);
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
