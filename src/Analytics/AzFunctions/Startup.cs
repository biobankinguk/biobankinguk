using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Biobanks.Data;
using Biobanks.Analytics.Core.Contracts;
using Biobanks.Analytics.Core;

namespace Biobanks.Analytics.AzFunctions
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

            services.AddHttpClient();

            // Register DbContext
            services.AddDbContext<BiobanksDbContext>(options => options
                .UseSqlServer(
                    config.GetConnectionString("Default")
                )
            );

            services.AddScoped<IAnalyticsReportGenerator, AnalyticsReportGenerator>();
            services.AddTransient<IBiobankReadService, BiobankReadService>();
            services.AddTransient<IGoogleAnalyticsReadService, GoogleAnalyticsReadService>();
        }

    }
}
