using Biobanks.IdentityModel.Services;
using Biobanks.IdentityModel.Types;

using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Text;

namespace Biobanks.IdentityTool.Commands.Runners
{
    internal class GenerateId
    {
        private readonly ILogger<GenerateId> _logger;
        private readonly CryptoService _crypto;

        public GenerateId(
            ILogger<GenerateId> logger,
            CryptoService crypto)
        {
            _logger = logger;
            _crypto = crypto;
        }

        public void Run(IConsole console, bool hash = false)
        {
            var outputFormat = OutputFormat.Base64Url;

            _logger.LogInformation(
                $"Generating as {{{nameof(outputFormat)}}}", outputFormat);

            var output = new StringBuilder().AppendLine(
                "Generation Results").AppendLine(
                "=========================");

            var id = _crypto.GenerateId(32, outputFormat);

            output.AppendLine(id);

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

