using AzFunctions;
using Microsoft.Extensions.Hosting;

// Configure the Host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(Startup.ConfigureServices);

// Run the app
await host.Build().RunAsync();