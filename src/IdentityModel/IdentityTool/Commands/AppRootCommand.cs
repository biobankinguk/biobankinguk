using System.CommandLine;

namespace Biobanks.IdentityTool.Commands
{
    internal class AppRootCommand : RootCommand
    {
        public AppRootCommand()
        {
            AddCommand(
                new Command("secrets", "Actions for working with secure secrets")
                {
                    new GenerateSecret("generate")
                });
        }
    }
}

