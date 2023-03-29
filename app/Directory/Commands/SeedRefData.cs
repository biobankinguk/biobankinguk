using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Commands.Helpers;
using Biobanks.Submissions.Api.Services.DataSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobanks.Submissions.Api.Commands;

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
        var connectionString = overrideConnectionString ?? config.GetConnectionString("Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s =>
          {
            s.AddSingleton(_ => logger)
              .AddSingleton(_ => config)
              .AddSingleton(_ => console);

            s.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString))
              .AddHttpClient();
            
            s.AddLogging()
              .AddIdentity<ApplicationUser, IdentityRole>()
              .AddEntityFrameworkStores<ApplicationDbContext>();

            s.AddTransient<BasicDataSeeder>()
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
