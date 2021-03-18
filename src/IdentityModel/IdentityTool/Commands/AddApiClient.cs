using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Biobanks.IdentityTool.Commands
{
    internal class AddApiClient : Command
    {
        public AddApiClient(string name)
            : base(name, "Add a new ApiClient to target BiobankingUK Directory Database")
        {
            new List<Option>
            {
                new(new[] {"--connection-string", "-c" },
                    "Database Connection String if not specified in Configuration")
                {
                    Argument = new Argument<string>()
                },

                new(
                    new[] {"--generate", "-g" },
                    "Generate new ApiClient credentials and output them to the console after adding them to the Database")
                {
                    Argument = new Argument<bool>()
                }
            }.ForEach(AddOption);

            Handler = CommandHandler.Create(
                async (IHost host, IConsole console, bool generate) =>
                {
                    var logger = host.Services.GetRequiredService<ILogger<AddApiClient>>();

                    // If we depend on conditionally injected services,
                    // we must check their availability prior to trying to use our resolved Runner

                    // Either try and resolve the desired service, e.g.
                    //     if(host.Services.GetService<BiobanksDbContext>() is null)

                    // or if that's expensive, otherwise check the known conditions, e.g.
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    if (config.GetConnectionString("Default") is null)
                    {
                        // TODO can we error as if parsing failed instead of a custom message?
                        // by massaging ParseResult perhaps?
                        logger.LogError(
                            // TODO: this is probably a reusable message :\
                            "No Connection String \"Default\" was specified. " +
                            "Please specify one in a Configuration source, or using the \"--connection-string\" option");
                        
                        return;
                    }

                    await host.Services.GetRequiredService<Runners.AddApiClient>().Run(console, generate);
                });
        }
    }
}

