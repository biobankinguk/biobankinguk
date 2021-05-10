using Microsoft.Extensions.Hosting;
using Biobanks.Publications.AzFunctions;

// Configure the Host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(Startup.ConfigureServices);

// Run the app
await host.Build().RunAsync();