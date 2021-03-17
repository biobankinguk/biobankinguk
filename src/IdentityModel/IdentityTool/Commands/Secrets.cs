using System.CommandLine;

namespace IdentityTool.Commands
{
    internal class Secrets : Command
    {
        public Secrets() : base("secrets", "Actions for working with secure secrets")
        {
            AddCommand(new GenerateSecret());
        }
    }
}

