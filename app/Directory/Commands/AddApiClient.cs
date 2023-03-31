using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using Biobanks.Data;
using Biobanks.Directory.Commands.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands;

internal class AddApiClient : Command
{
  public AddApiClient(string name)
    : base(name, "Add a new ApiClient to target BiobankingUK Directory Database")
  {
    var argBiobankIds = new Argument<List<int>>(
      "biobankIds",
      "The Database Record Ids for Biobanks to associate this Client with. No Biobanks will result in a SuperAdmin.")
    {
      Arity = ArgumentArity.ZeroOrMore
    };
    Add(argBiobankIds);

    var optConnectionString = new Option<string>(new[] { "--connection-string", "-c" },
      "Database Connection String if not specified in Configuration");
    Add(optConnectionString);

    var optGenerate = new Option<bool>(new[] { "--generate", "-g" },
      "Generate new ApiClient credentials and output them to the console after adding them to the Database");
    Add(optGenerate);

    var optClientId = new Option<string>(new[] { "--client-id", "-i" },
      "The Client Identifier. Required if --generate is not specified.");
    Add(optClientId);

    var optClientSecret = new Option<string>(new[] { "--client-secret", "-s" },
      "The Client Secret (not hashed). Required if --generate is not specified.");
    Add(optClientSecret);

    var optClientName = new Option<string>(
      new[] { "--client-name", "-n" },
      "The Client Name. If omitted the Client ID will be used as a name.");
    Add(optClientName);

    this.SetHandler(
      async (
        context,
        generate,
        biobankIds,
        clientId,
        clientSecret,
        clientName,
        overrideConnectionString) =>
      {
        // Get some extra dependencies off the context
        var logger = ServiceProviderServiceExtensions.GetRequiredService<ILoggerFactory>(context.BindingContext);
        var config = ServiceProviderServiceExtensions.GetRequiredService<IConfiguration>(context.BindingContext);
        var console = context.Console;

        // figure out the connection string from the option, or config
        var connectionString = overrideConnectionString ?? config.GetConnectionString("Default");

        // Configure DI and run the command handler
        await this
          .ConfigureServices(s => ServiceCollectionServiceExtensions.AddSingleton(s, _ => logger)
            .AddSingleton(_ => config)
            .AddSingleton<IConsole>(_ => console)
            .AddDbContext<ApplicationDbContext>(
              o => o.UseNpgsql(
                connectionString,
                pgo => pgo.EnableRetryOnFailure()))
            .AddTransient<Runners.AddApiClient>())
          .GetRequiredService<Runners.AddApiClient>()
          .Run(generate, biobankIds ?? new(), clientId, clientSecret, clientName);
      },
      Bind.FromServiceProvider<InvocationContext>(),
      optGenerate,
      argBiobankIds,
      optClientId,
      optClientSecret,
      optClientName,
      optConnectionString);
  }
}
