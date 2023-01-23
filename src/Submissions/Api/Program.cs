using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Biobanks.Submissions.Api.Commands.Helpers;
using Biobanks.Submissions.Api.Startup.Cli;
using Biobanks.Submissions.Api.Startup.Web;

// Global App Startup stuff here 

// Initialise the command line parser and run the appropriate entrypoint
await new CommandLineBuilder(new CliEntrypoint())
  .UseDefaults()
  .UseRootCommandBypass(args, WebEntrypoint.Run)
  .UseCliHostDefaults(args)
  .Build()
  .InvokeAsync(args);
