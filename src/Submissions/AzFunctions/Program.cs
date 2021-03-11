using AzFunctions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using System.Reflection;

// Configure the Host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()

    .ConfigureAppConfiguration(b =>
        b.AddUserSecrets(Assembly.GetAssembly(typeof(Startup))))

    // Configure dependencies, like ASP.NET Core
    .ConfigureServices(Startup.ConfigureServices);

// Run the app
await host.Build().RunAsync();