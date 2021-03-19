using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Biobanks.IdentityTool.Commands
{
    internal class GenerateId : Command
    {
        public GenerateId(string name)
            : base(name, "Generates a secure unique identifier in Base64Url format, suitable for use as a secret.")
        {
            new List<Option>
            {
                new(new[] { "-s", "--sha", "--hash" },
                "Also output a SHA256 hash of the ID in Base64Url format.")
                {
                    Argument = new Argument<bool>()
                },
            }.ForEach(AddOption);

            Handler = CommandHandler.Create(
                (IHost host, IConsole console, bool hash) =>
                    host.Services.GetRequiredService<Runners.GenerateId>()
                        .Run(console, hash));
        }
    }
}

