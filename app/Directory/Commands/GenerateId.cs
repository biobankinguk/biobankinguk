using System.CommandLine;
using Biobanks.Directory.Commands.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Biobanks.Directory.Commands;

public class GenerateId : Command
{
  public GenerateId(string name)
    : base(name, "Generates a secure unique identifier in Base64Url format, suitable for use as a secret.")
  {
    var optHash = new Option<bool>(new[] { "-s", "--sha", "--hash" },
      "Also output a SHA256 hash of the ID in Base64Url format.");
    Add(optHash);

    this.SetHandler(
      (logger, console, hash) =>
      {
        this.ConfigureServices(s =>
            ServiceCollectionServiceExtensions.AddSingleton<IConsole>(s, _ => logger).AddSingleton<ILoggerFactory>(_ => console).AddTransient<Runners.GenerateId>())
          .GetRequiredService<Runners.GenerateId>().Run(hash);
      },
      Bind.FromServiceProvider<IConsole>(),
      Bind.FromServiceProvider<ILoggerFactory>(),
      optHash);
  }
}
