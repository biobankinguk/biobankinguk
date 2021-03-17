using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace IdentityTool.Commands
{
    public enum Encoding
    {
        Hex,
        Base64,
        Base64Url
    }

    internal class GenerateSecret : Command
    {
        public GenerateSecret() : base("generate", "Generates a secure secret in a given output format.")
        {
            // define options
            var hashOption = new Option(
                new[] { "-s", "--sha" },
                "Also output a SHA256 hashed version of the generated secret.")
            {
                Argument = new Argument<bool>()
            };

            var encodingOption = new Option(
                new[] { "-e", "--encoding" },
                "Encode the output")
            {
                Argument = new Argument<Encoding>()
            };

            // add options
            AddOption(hashOption);
            AddOption(encodingOption);

            // Command Handler
            Handler = CommandHandler.Create((IHost host) =>
            {
                // do some "model binding" manually
                // the price we pay, for dependency injection
                var command = host.Services.GetRequiredService<ParseResult>();
                var hash = command.FindResultFor(hashOption)?.GetValueOrDefault<bool>() ?? false;
                var encoding = command.FindResultFor(encodingOption)?.GetValueOrDefault<Encoding>() ?? Encoding.Hex;

                host.Services.GetRequiredService<GenerateSecretHandler>()
                    .Run(hash, encoding);
            });
        }
    }

    internal class GenerateSecretHandler
    {
        private readonly ILogger<GenerateSecret> _logger;

        public GenerateSecretHandler(ILogger<GenerateSecret> logger)
        {
            _logger = logger;
        }

        public void Run(bool hash, Encoding encoding)
        {
            var secret = "LOL";
            _logger.LogInformation($"Generated Secret: {secret}");
            Console.WriteLine(secret);
            if (hash)
            {
                _logger.LogInformation($"Hashed Secret: KEK");
                Console.WriteLine(secret);
            }


            switch (encoding)
            {
                case Encoding.Base64:
                    _logger.LogInformation(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("LOL")));
                    break;
                default:
                    _logger.LogInformation($"no specific encoding");
                    break;
            }
        }
    }
}

