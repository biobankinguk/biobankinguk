using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Biobanks.Data;
using Analytics.Services.Contracts;
using Analytics.Services;

namespace Analytics.AnalyticsAzureFunctions
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

            // In-Memory Cache - Manually Called As Not Called By A Parent Service
            services.AddMemoryCache();

            // Register DbContext
            services.AddDbContext<BiobanksDbContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(
                    config.GetConnectionString("Default")
                ),
                ServiceLifetime.Transient
            );

            services.AddScoped<IAnalyticsReportGenerator, AnalyticsReportGenerator>();
            services.AddTransient<IBiobankWebService, BiobankWebService>();
            services.AddTransient<IGoogleAnalyticsReadService, GoogleAnalyticsReadService>();
        }

    }
}
