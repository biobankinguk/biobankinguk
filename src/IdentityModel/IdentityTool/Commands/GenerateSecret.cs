using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

using static IdentityModel.CryptoRandom;

namespace Biobanks.IdentityTool.Commands
{
    internal class GenerateSecret : Command
    {
        public GenerateSecret(string name)
            : base(name, "Generates a secure secret in a given output format.")
        {
            new List<Option>
            {
                new(new[] { "-s", "--sha" },
                "Also output a SHA256 hashed version of the generated secret.")
                {
                    Argument = new Argument<bool>()
                },
                new(new[] { "-f", "--output-format" },
                "Output format for the generated secret")
                {
                    Argument = new Argument<OutputFormat>()
                }
            }.ForEach(AddOption);

            Handler = CommandHandler.Create(
                (IHost host, IConsole console, bool sha, OutputFormat encoding) =>
                    host.Services.GetRequiredService<Runners.GenerateSecret>()
                        .Run(console, sha, encoding));
        }
    }
}

