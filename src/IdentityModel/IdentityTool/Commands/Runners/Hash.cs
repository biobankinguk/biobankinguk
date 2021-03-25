using Biobanks.IdentityModel.Extensions;
using Biobanks.IdentityModel.Types;

using ConsoleTableExt;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.CommandLine;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class Hash
    {
        private readonly ILogger<Hash> _logger;

        public Hash(
            ILogger<Hash> logger)
        {
            _logger = logger;
        }

        public void Run(IConsole console, string input)
        {
            _logger.LogInformation(
                $"Hashing {{{nameof(input)}}} with SHA256 as {{hashFormat}}",
                input,
                OutputFormat.Base64Url);

            var outputRows = new List<List<object>>
            {
                new() { "Input", input },
                new() { "SHA256", input.Sha256() }
            };

            console.Out.Write(ConsoleTableBuilder
                .From(outputRows)
                .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
                .Export()
                .ToString());
        }
    }
}

