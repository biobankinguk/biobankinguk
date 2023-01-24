using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobanks.Submissions.Api.Commands.Helpers;

public static class CommandLineExtensions
{
  /// <summary>
  /// Adds middleware that will bypass System.CommandLine for the root command
  /// and instead simply pass through `args` and execute the provided Action
  /// </summary>
  /// <param name="cli">The CommandLineBuilder instance</param>
  /// <param name="args">the cli arguments to pass through when overriding the root command</param>
  /// <param name="newRoot">The async Func to execute instead of the root command</param>
  /// <returns></returns>
  public static CommandLineBuilder UseRootCommandBypass(this CommandLineBuilder cli, string[] args,
    Func<string[],Task> newRoot) =>
    cli.AddMiddleware(async (context, next) =>
    {
      if (context.ParseResult.CommandResult.Command == cli.Command)
      {
        await newRoot.Invoke(args);
      }
      // otherwise, carry on as normal
      else await next(context);
    });

  /// <summary>
  /// Get some default behaviours such as config loading and basic
  /// global services like logging from the .NET Generic Host
  /// and use them for the CLI
  /// </summary>
  /// <param name="cli"></param>
  /// <param name="args"></param>
  /// <returns></returns>
  public static CommandLineBuilder UseCliHostDefaults(this CommandLineBuilder cli, string[] args)
  {
    // in a shockingly similar pattern to aspnet,
    // we use a generic host just to load config and bootstrap things
    // never intending to use it to run anything <3
    var bootstrapHost = Host.CreateDefaultBuilder(args).Build();
    
    // In this case we once the host has been created, we inject configuration
    // and some other default services (e.g. logging)
    // into the System.CommandLine BindingContext for use in commands.
    return cli.AddMiddleware(async (context, next) =>
    {
      // Logging Services
      context.BindingContext.AddService(s => bootstrapHost.Services.GetRequiredService<ILoggerFactory>());
      
      // Host Configuration
      context.BindingContext.AddService(s => bootstrapHost.Services.GetRequiredService<IConfiguration>());

      await next(context);
    });
  }

  
}
