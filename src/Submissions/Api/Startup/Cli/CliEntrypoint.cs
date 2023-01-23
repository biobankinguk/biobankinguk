using System.CommandLine;
using Biobanks.Submissions.Api.Commands;

namespace Biobanks.Submissions.Api.Startup.Cli;

public class CliEntrypoint : RootCommand
{
  public CliEntrypoint() : base("Biobankinguk Directory CLI")
  {
    // Add Commands here

    AddCommand(new Hash("hash"));
  }
}
