using Biobanks.Data;
using Biobanks.Submissions.Core.Config;
using Biobanks.Submissions.Core.Services;
using Biobanks.Submissions.Core.Services.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AzFunctions
{
    class Startup
    {

        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {

            var config = context.Configuration;

            // Application Insights Telemtry
            var appInsightsKey = config.GetValue<string>("APPINSIGHTS_INSTRUMENTATIONKEY");

            if (!string.IsNullOrEmpty(appInsightsKey))
            {
                services.AddApplicationInsightsTelemetry(appInsightsKey);
            }

            // Configuration
            services.Configure<ExpiryConfigModel>(c =>
            {
                c.ExpiryDays = config.GetValue("expiryDays", 30);
            });

            // In-Memory Cache - Manually Called As Not Called By A Parent Service
            services.AddMemoryCache();

            // AutoMapper Profiles
            services.AddAutoMapper(
                typeof(Biobanks.Submissions.Core.MappingProfiles.DiagnosisProfile));

            // Register DbContext
            services.AddDbContext<BiobanksDbContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(
                    config.GetConnectionString("DefaultConnection")
                ),
                ServiceLifetime.Transient
            );

            // Cloud Services
            services.AddTransient(s =>
                CloudStorageAccount.Parse(
                    config.GetConnectionString("AzureQueueConnection")
                )
            );

            // Storage Services
            services.AddTransient<IBlobReadService, AzureBlobReadService>();
            services.AddTransient<IBlobWriteService, AzureBlobWriteService>();
            services.AddTransient<IQueueWriteService, AzureQueueWriteService>();

            // Core Data Services
            services.AddTransient<ISampleWriteService, SampleWriteService>();
            services.AddTransient<ITreatmentWriteService, TreatmentWriteService>();
            services.AddTransient<IDiagnosisWriteService, DiagnosisWriteService>();
            services.AddTransient<ISampleValidationService, SampleValidationService>();
            services.AddTransient<IReferenceDataReadService, ReferenceDataReadService>();
            services.AddTransient<ITreatmentValidationService, TreatmentValidationService>();
            services.AddTransient<IDiagnosisValidationService, DiagnosisValidationService>();

            // Submission Services
            services.AddTransient<IErrorService, ErrorService>();
            services.AddTransient<ISubmissionService, SubmissionService>();
            services.AddTransient<ISubmissionExpiryService, SubmissionExpiryService>();
        }
    }
}
