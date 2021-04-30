using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Analytics.AnalyticsAzureFunctions;

// Configure the Host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(Startup.ConfigureServices);

// Run the app
await host.Build().RunAsync();


