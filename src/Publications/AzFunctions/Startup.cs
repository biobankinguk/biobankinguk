using Biobanks.Data;
using Biobanks.Publications.Core.Services;
using Biobanks.Publications.Core.Services.Contracts;
using Biobanks.Publications.Core.Services.Hosted;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.Publications.AzFunctions
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
                .UseSqlServer(
                    config.GetConnectionString("Default")
                )
            );

            //DI
            services.AddHttpClient();
            services.AddTransient<IEpmcService, EpmcWebService>();
            services.AddTransient<IPublicationService, PublicationService>();
            services.AddTransient<IAnnotationService, AnnotationService>();
            services.AddTransient<IBiobankReadService, BiobankReadService>();
            services.AddTransient<FetchPublicationsService>();
            services.AddTransient<FetchAnnotationsService>();
        }
    }
}
