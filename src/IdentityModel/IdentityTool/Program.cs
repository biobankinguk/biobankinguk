using IdentityTool;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Reflection;

var cmd = new CommandLineBuilder(new MyRootCommand())
    .UseDefaults()
    .UseHost(hostArgs =>
        Host.CreateDefaultBuilder(hostArgs)
            .ConfigureAppConfiguration(b =>
                b.AddUserSecrets(Assembly.GetAssembly(typeof(MyRootCommand))))
            .UseSerilog((context, services, loggerConfig) => loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()))
    .Build();

await cmd.InvokeAsync(args);