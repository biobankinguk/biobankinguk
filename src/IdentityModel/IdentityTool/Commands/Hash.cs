using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Biobanks.IdentityTool.Commands
{
    internal class Hash : Command
    {
        public Hash(string name)
            : base(name, "Hash a string input (such as a secret) per BiobankingUK expectations")
        {
            new List<Argument>
            {
                new Argument<string>("input", "The input string to hash")
            }.ForEach(AddArgument);

            Handler = CommandHandler.Create(
                (IHost host, IConsole console, string input) =>
                    host.Services.GetRequiredService<Runners.Hash>()
                        .Run(console, input));
        }
    }
}

