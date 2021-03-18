using Biobanks.IdentityTool;
using Biobanks.IdentityTool.Commands;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using Serilog;

using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Reflection;

var appAssembly = Assembly.GetAssembly(typeof(Startup));

var cmd = new CommandLineBuilder(new AppRootCommand())
    .UseDefaults()
    .UseHost(hostArgs =>
        Host.CreateDefaultBuilder(hostArgs)
            .ConfigureAppConfiguration(b =>
                b.AddUserSecrets(appAssembly))
            .UseSerilog((context, _, loggerConfig) => loggerConfig
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext())
            .ConfigureServices(Startup.ConfigureServices))
            
    .Build();

await cmd.InvokeAsync(args);