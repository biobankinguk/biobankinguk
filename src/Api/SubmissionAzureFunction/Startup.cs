using AutoMapper;
using Biobanks.SubmissionAzureFunction.Config;
using Biobanks.SubmissionAzureFunction.MappingProfiles;
using Biobanks.SubmissionAzureFunction.Services;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Biobanks.LegacyData;

[assembly: FunctionsStartup(typeof(Biobanks.SubmissionAzureFunction.Startup))]

namespace Biobanks.SubmissionAzureFunction
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            // Application Insights Telemtry
            var appInsightsKey = _configuration.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                builder.Services.AddApplicationInsightsTelemetry(appInsightsKey);
            }

            // Configuration
            builder.Services.Configure<ConfigModel>(config =>
            {
                config.ExpiryDays = _configuration.GetValue<int>("expiryDays", 30);
            });

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
            builder.Services.AddDbContext<BiobanksDbContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                ),
                ServiceLifetime.Transient
            );

            // Cloud Services
            builder.Services.AddTransient(s =>
                CloudStorageAccount.Parse(
                    _configuration.GetConnectionString("AzureQueueConnection")
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
            builder.Services.AddTransient<ISubmissionExpiryService, SubmissionExpiryService>();
        }
    }
}