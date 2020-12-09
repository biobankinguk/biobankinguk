using System;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.ExpiryJob.Services;
using Biobanks.ExpiryJob.Services.Contracts;
using Biobanks.ExpiryJob.Settings;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Biobanks.ExpiryJob
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
                .ConfigureWebJobs(b =>
                {
                    b.AddAzureStorageCoreServices()
                        .AddAzureStorage()
                        .AddServiceBus()
                        .AddEventHubs();
                })
                .ConfigureAppConfiguration(b =>
                {
                    // Adding command line as a configuration source
                    b.AddCommandLine(args);
                })
                .ConfigureLogging((context, b) =>
                {
                    b.SetMinimumLevel(LogLevel.Debug);
                    b.AddConsole();

                    // If this key exists in any config, use it to enable App Insights
                    var appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
                    if (!string.IsNullOrEmpty(appInsightsKey))
                    {
                        b.AddApplicationInsights(o => o.InstrumentationKey = appInsightsKey);
                    }
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<SubmissionsDbContext>(opts =>
                        opts.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                    services.Configure<SettingsModel>(context.Configuration.GetSection("Settings"));
                    services.AddTransient<ISubmissionService, SubmissionService>();
                    services.AddSingleton<Functions, Functions>();

                    // job activator, required in webjobs sdk 3+
                    services.AddSingleton<IJobActivator>(new WebJobsActivator(services.BuildServiceProvider()));
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
