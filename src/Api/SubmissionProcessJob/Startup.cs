using AutoMapper;
using Biobanks.Common.Data;
using Biobanks.SubmissionJob.MappingProfiles;
using Biobanks.SubmissionJob.Services;
using Biobanks.SubmissionProcessJob.MappingProfiles;
using Biobanks.SubmissionProcessJob.Services.Contracts;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;

[assembly: FunctionsStartup(typeof(Biobanks.SubmissionProcessJob.Startup))]

namespace Biobanks.SubmissionProcessJob
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            // TODO: Implement These Configuration Options
            //var builder = new HostBuilder()
            //    .UseEnvironment(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            //    .ConfigureWebJobs(b =>
            //    {
            //        b.AddAzureStorageCoreServices()
            //            .AddAzureStorage(q => q.BatchSize = 1)
            //            .AddServiceBus()
            //            .AddEventHubs();
            //    })
            //    .ConfigureAppConfiguration(b =>
            //    {
            //                    // Adding command line as a configuration source
            //                    b.AddCommandLine(args);
            //    })
            //    .ConfigureLogging((context, b) =>
            //    {
            //        b.SetMinimumLevel(LogLevel.Debug);
            //        b.AddConsole();

            //                    // If this key exists in any config, use it to enable App Insights
            //                    var appInsightsKey = context.Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
            //        if (!string.IsNullOrEmpty(appInsightsKey))
            //        {
            //            b.AddApplicationInsights(o => o.InstrumentationKey = appInsightsKey);
            //        }
            //    })


            // In-Memory Cache - Manually Called As Not Called By A Parent Service
            builder.Services.AddMemoryCache();

            // AutoMapper Profiles
            builder.Services.AddAutoMapper(s =>
            {
                s.AddProfile<DiagnosisProfile>();
                s.AddProfile<SampleProfile>();
                s.AddProfile<TreatmentProfile>();
            });

            // Register DbContext
            builder.Services.AddDbContext<SubmissionsDbContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                ),
                ServiceLifetime.Transient
            );

            // Cloud Services
            builder.Services.AddTransient(s =>
                CloudStorageAccount.Parse(
                    _configuration.GetConnectionString("")
                )
            );

            builder.Services.AddTransient<IBlobReadService, AzureBlobReadService>();
            builder.Services.AddTransient<IBlobWriteService, AzureBlobWriteService>();
            builder.Services.AddTransient<IQueueWriteService, AzureQueueWriteService>();

            // Core Data Services
            builder.Services.AddTransient<ISampleWriteService, SampleWriteService>();
            builder.Services.AddTransient<ITreatmentWriteService, TreatmentWriteService>();
            builder.Services.AddTransient<IDiagnosisWriteService, DiagnosisWriteService>();
            builder.Services.AddTransient<ISampleValidationService, SampleValidationService>();
            builder.Services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            builder.Services.AddTransient<ITreatmentValidationService, TreatmentValidationService>();
            builder.Services.AddTransient<IDiagnosisValidationService, DiagnosisValidationService>();

            // Submission Services
            builder.Services.AddTransient<ISubmissionStatusService, SubmissionStatusService>();
        }
    }
}