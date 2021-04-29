using AzFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

// Configure the Host
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, b) =>
    b.AddJsonFile(
        Path.Combine(
            context.HostingEnvironment.ContentRootPath,
            "../../../Core/Settings/LegacyStorageTemperatures.json"),
            optional: false)
    .AddJsonFile(
        Path.Combine(
            context.HostingEnvironment.ContentRootPath,
            "../../../Core/Settings/LegacyMaterialTypes.json"),
            optional: false))
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(Startup.ConfigureServices);

// Run the app
await host.Build().RunAsync();

