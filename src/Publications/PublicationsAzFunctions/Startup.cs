using Biobanks.Data;
using Biobanks.Publications.Services;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Publications.Services.Hosted;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace PublicationsAzFunctions
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

            // Register DbContext
            services.AddDbContext<BiobanksDbContext>(options => options
                .EnableSensitiveDataLogging()
                .UseSqlServer(
                    config.GetConnectionString("Default")
                ),
                ServiceLifetime.Transient
            );

            //DI
            services.AddHttpClient();
            services.AddScoped<IEpmcService, EpmcWebService>();
            services.AddScoped<IPublicationService, PublicationService>();
            services.AddScoped<IAnnotationService, AnnotationService>();
            services.AddScoped<IBiobankReadService, BiobankReadService>();
            services.AddTransient<FetchPublicationsService>();
            services.AddTransient<FetchAnnotationsService>();
            //services.AddLogging();
        }
    }
}
