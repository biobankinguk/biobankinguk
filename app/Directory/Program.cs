using System;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Biobanks.Directory.Commands.Helpers;
using Biobanks.Directory.Startup.Cli;
using Biobanks.Directory.Startup.EfCoreMigrations;
using Biobanks.Directory.Startup.Web;

// Enable EF Core tooling to get a DbContext configuration
EfCoreMigrations.BootstrapDbContext(args);

// Global App Startup stuff here

// See https://www.npgsql.org/doc/types/datetime.html#timestamps-and-timezones
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Initialise the command line parser and run the appropriate entrypoint
await new CommandLineBuilder(new CliEntrypoint())
  .UseDefaults()
  .UseRootCommandBypass(args, WebEntrypoint.Run)
  .UseCliHostDefaults(args)
  .Build()
  .InvokeAsync(args);
