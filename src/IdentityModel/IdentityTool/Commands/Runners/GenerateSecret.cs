using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Text;

using static IdentityModel.CryptoRandom;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class GenerateSecret
    {
        private readonly ILogger<GenerateSecret> _logger;

        public GenerateSecret(ILogger<GenerateSecret> logger)
        {
            _logger = logger;
        }

        public void Run(IConsole console, bool hash, OutputFormat encoding)
        {
            _logger.LogInformation("Generating secret...");


            var output = new StringBuilder().AppendLine(
                "Secret Generation Results").AppendLine(
                "=========================");

            var secret = "LOL";

            output.Append("Raw:    ").AppendLine(secret);

            // if (hash) output.AppendLine("Hashed: KEK");

            //switch (encoding)
            //{
            //    case EncodingFormat.Base64:
            //        var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes("LOL"));
            //        output.Append("Base64: ").AppendLine(encoded);
            //        break;
            //    default:
            //        break;
            //}

            console.Out.Write(output.ToString());
        }
    }
}

