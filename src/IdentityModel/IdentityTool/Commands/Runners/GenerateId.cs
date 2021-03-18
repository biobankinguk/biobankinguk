using Biobanks.IdentityModel.Extensions;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Types;

using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Text;

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
            var outputFormat = OutputFormat.Base64Url;

            var output = new StringBuilder()
                .AppendLine().AppendLine(
                "|==============|===");

            // Generate the ID
            _logger.LogInformation(
                $"Generating as {{{nameof(outputFormat)}}}",
                outputFormat);

            var id = Crypto.GenerateId(32, outputFormat);

            output.Append("| ID:          | ")
                .AppendLine(id);

            // Generate the Hash
            if (hash)
            {
                _logger.LogInformation(
                    "Hashing with SHA256 as {hashFormat}",
                    OutputFormat.Base64Url);

                output.AppendLine("|--------------|---")
                    .Append("| SHA256 Hash: | ")  
                    .AppendLine(id.Sha256());
            }

            output.AppendLine("|==============|===").AppendLine();
            console.Out.Write(output.ToString());
        }
    }
}

