using Biobanks.IdentityModel.Extensions;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Types;

using ConsoleTableExt;

using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.CommandLine;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class GenerateId
    {
        private readonly ILogger<GenerateId> _logger;

        public GenerateId(
            ILogger<GenerateId> logger)
        {
            _logger = logger;
        }

        public void Run(IConsole console, bool hash = false)
        {
            const OutputFormat outputFormat = OutputFormat.Base64Url;

            var outputRows = new List<List<object>>();

            // Generate the ID
            _logger.LogInformation(
                $"Generating as {{{nameof(outputFormat)}}}",
                outputFormat);

            var id = Crypto.GenerateId(32, outputFormat);

            outputRows.Add(new() { "ID", id });

            // Generate the Hash
            if (hash)
            {
                _logger.LogInformation(
                    "Hashing with SHA256 as {hashFormat}",
                    OutputFormat.Base64Url);

                outputRows.Add(new() { "SHA256", id.Sha256() });
            }

            // Output
            console.Out.Write(ConsoleTableBuilder
                .From(outputRows)
                .WithCharMapDefinition(CharMapDefinition.FramePipDefinition)
                .Export()
                .ToString());
        }
    }
}

