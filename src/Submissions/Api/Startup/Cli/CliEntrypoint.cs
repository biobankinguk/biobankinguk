using System.CommandLine;
using Biobanks.Submissions.Api.Commands;

namespace Biobanks.Submissions.Api.Startup.Cli;

public class CliEntrypoint : RootCommand
{
  public CliEntrypoint() : base("BiobankingUK Directory CLI")
  {
    AddGlobalOption(new Option<string>(new[] { "--environment", "-e" }));

    // Add Commands here

    AddCommand(new Command("crypto", "Actions for working with secure identifiers")
    {
      new GenerateId("generate-id"),
      new Hash("hash")
    });

    AddCommand(new Command("api-clients", "Actions for managing BiobankingUK ApiClients")
    {
      new AddApiClient("add")
    });

    AddCommand(new Command("ref-data", "Actions for managing BiobankingUK Reference Data")
    {
      new SeedRefData("seed")
    });

    AddCommand(new Command("users", "Actions for managing BiobankingUK Users")
    {
      new AddUser("add"),
      new ManageUserRoles("roles"),
      new ListRoles("list-roles")
    });
  }
}
