using System.CommandLine;

namespace Biobanks.IdentityTool.Commands
{
    internal class AppRootCommand : RootCommand
    {
        public AppRootCommand()
        {
            AddCommand(new Command("crypto", "Actions for working with secure identifiers")
                {
                    new GenerateId("generate-id"),
                    new Hash("hash")
                });

            AddCommand(new Command("api-clients", "Actions for managing BiobankingUK ApiClients")
            {
                new AddApiClient("add")
            });
        }
    }
}