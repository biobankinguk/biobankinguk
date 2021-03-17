
using System.CommandLine;

namespace IdentityTool.Commands
{
    internal class AppRootCommand : RootCommand
    {
        public AppRootCommand()
        {
            AddCommand(new Secrets());
        }
    }
}

