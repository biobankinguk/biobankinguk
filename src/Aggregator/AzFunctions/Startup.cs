using Biobanks.Aggregator.Core;
using Biobanks.Aggregator.Core.Services;
using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.Aggregator.AzFunctions
{
    class Startup
    {
        internal static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Register DbContext
            services.AddDbContext<BiobanksDbContext>(options => options
                .UseSqlServer(
                    context.Configuration.GetConnectionString("Default")
                )
            );

            // Services
            services.AddTransient<IAggregationService, AggregationService>();
            services.AddTransient<IReferenceDataService, ReferenceDataService>();
            services.AddTransient<IOrganisationService, OrganisationService>();
            services.AddTransient<ICollectionService, CollectionService>();
            services.AddTransient<ISampleService, SampleService>();

            services.AddSingleton<AggregationTask>();
        }
    }
}
