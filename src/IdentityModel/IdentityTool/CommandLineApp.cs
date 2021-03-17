using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace IdentityTool
{
    internal class MyRootCommand : RootCommand
    {
        public MyRootCommand()
        {
            AddCommand(new MyCommand());
        }
    }

    internal class MyCommand : Command
    {
        public MyCommand() : base("say-hello")
        {
            var arg = new Argument<string>("VALUE")
            {
                Description = "A value",
                Arity = ArgumentArity.ZeroOrOne
            };

            Handler = CommandHandler.Create((IHost host) =>
                {
                    var logger = host.Services
                      .GetService<ILogger<MyRootCommand>>();


                    logger.LogInformation("Hello world!");

                    var cmd = host.Services.GetService<ParseResult>();
                    var value = cmd.FindResultFor(arg)?.GetValueOrDefault<string>();

                    if (!string.IsNullOrWhiteSpace(value))
                        logger.LogInformation($"Argument passed: {{{nameof(value)}}}", value);
                });

            AddArgument(arg);
        }
    }
}

