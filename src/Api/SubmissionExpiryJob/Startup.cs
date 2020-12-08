using Biobanks.Common.Data;
using Biobanks.SubmissionExpiryJob.Services;
using Biobanks.SubmissionExpiryJob.Services.Contracts;
using Biobanks.SubmissionExpiryJob.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Biobanks.SubmissionExpiryJob.Startup))]

namespace Biobanks.SubmissionExpiryJob
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

            // Settings From Configuration
            builder.Services.Configure<SettingsModel>(options =>
            {
                options.ExpiryDays = _configuration.GetValue<int>("expiryDays", 30);
            });

            // Register DbContext
            builder.Services.AddDbContext<SubmissionsDbContext>(options =>
                options.UseSqlServer(
                    _configuration.GetConnectionString("DefaultConnection")
                )
            );

            // Dependency Injection
            builder.Services.AddScoped<ISubmissionExpiryService, SubmissionExpiryService>();
        }
    }
}