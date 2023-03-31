using System.CommandLine;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Commands.Helpers;
using Biobanks.Directory.Services.DataSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands;

internal class SeedRefData : Command
{
  public SeedRefData(string name)
    : base(name, "Seed Reference Data into empty tables from provided JSON files")
  {
    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

    var optDataDirectory = new Option<string>(new[] { "--data-directory", "-d" },
      "The directory from which to load source data JSON files");
    Add(optDataDirectory);

    // TODO: UN countries option?

    // TODO: which tables option

    // TODO: replace existing data option?

    this.SetHandler(
      async (
        logger, config, console,
        overrideConnectionString,
        dataDirectory) =>
      {
        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? ConfigurationExtensions.GetConnectionString(config, "Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s =>
          {
            ServiceCollectionServiceExtensions.AddSingleton<ILoggerFactory>(s, _ => logger)
              .AddSingleton<IConfiguration>(_ => config)
              .AddSingleton<IConsole>(_ => console);

            EntityFrameworkServiceCollectionExtensions.AddDbContext<ApplicationDbContext>(s, o => NpgsqlDbContextOptionsBuilderExtensions.UseNpgsql(o, connectionString))
              .AddHttpClient();
            
            LoggingServiceCollectionExtensions.AddLogging(s)
              .AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            ServiceCollectionServiceExtensions.AddTransient<BasicDataSeeder>(s)
              .AddTransient<CustomRefDataSeeder>()
              .AddTransient<FixedRefDataSeeder>()
              .AddTransient<Runners.SeedRefData>();
          })
          .GetRequiredService<Runners.SeedRefData>()
          .Run(dataDirectory ?? "./data");
      },
      Bind.FromServiceProvider<ILoggerFactory>(),
      Bind.FromServiceProvider<IConfiguration>(),
      Bind.FromServiceProvider<IConsole>(),
      optConnectionString, optDataDirectory);
  }
}
