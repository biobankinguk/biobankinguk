using System;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Data;
using Biobanks.SubmissionJob.Services;
using Biobanks.SubmissionJob.Services.Contracts;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Biobanks.SubmissionJob
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
                        .AddAzureStorage(q => q.BatchSize = 1)
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
                    services.AddAutoMapper();

                    services.AddMemoryCache();

                    services.AddDbContext<SubmissionsDbContext>(opts =>
                    {
                        opts.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection"));
                        opts.EnableSensitiveDataLogging();
                    }, ServiceLifetime.Transient);

                    // cloud services
                    services.AddTransient(s =>
                        CloudStorageAccount.Parse(
                            context.Configuration.GetConnectionString("AzureQueueConnectionString")));
                    services.AddTransient<IBlobReadService, AzureBlobReadService>();
                    services.AddTransient<IBlobWriteService, AzureBlobWriteService>();
                    services.AddTransient<IQueueWriteService, AzureQueueWriteService>();

                    // core data services
                    services.AddTransient<ISampleWriteService, SampleWriteService>();
                    services.AddTransient<ITreatmentWriteService, TreatmentWriteService>();
                    services.AddTransient<IDiagnosisWriteService, DiagnosisWriteService>();
                    services.AddTransient<ISampleValidationService, SampleValidationService>();
                    services.AddTransient<ITreatmentValidationService, TreatmentValidationService>();
                    services.AddTransient<IDiagnosisValidationService, DiagnosisValidationService>();
                    services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();

                    // submission services
                    services.AddTransient<ISubmissionStatusService, SubmissionStatusService>();

                    // Main function with queue triggers
                    services.AddTransient<Functions, Functions>();
                });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
