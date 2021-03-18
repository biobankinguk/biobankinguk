using IdentityTool.Types;

using Microsoft.Extensions.Logging;

using System;
using System.CommandLine;
using System.Text;

namespace IdentityTool.Commands.Runners
{
    internal class GenerateSecret
    {
        private readonly ILogger<GenerateSecret> _logger;

        public GenerateSecret(ILogger<GenerateSecret> logger)
        {
            _logger = logger;
        }

        public void Run(IConsole console, bool hash, EncodingFormat encoding)
        {
            // Use a stringbuilder to progressively build the output
            // and only render it at the end so it's not interspersed with logging throughout
            var output = new StringBuilder().AppendLine(
                "Secret Generation Results").AppendLine(
                "=========================");

            var secret = "LOL";

            _logger.LogInformation($"Generated Secret: {{{nameof(secret)}}}", secret);
            output.Append("Raw:    ").AppendLine(secret);

            if (hash)
            {
                _logger.LogInformation("Hashed Secret: {hash}", "KEK");
                output.AppendLine("Hashed: KEK");
            }


            switch (encoding)
            {
                case EncodingFormat.Base64:
                    var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("LOL"));
                    _logger.LogInformation(
                        $"Encoded (Base64): {{{nameof(encoded)}}}", encoded);
                    output.Append("Base64: ").AppendLine(encoded);
                    break;
                default:
                    _logger.LogInformation("no specific encoding requested");
                    break;
            }

            // Render the output
            console.Out.Write(output.ToString());
        }
    }
}

