using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Analytics.Services.Contracts;
using Analytics.Services;
using Analytics.Data;
using Analytics.Data.Repositories;
using Analytics.Data.Entities;

[assembly: FunctionsStartup(typeof(Analytics.AnalyticsAzureFunctions.Startup))]

namespace Analytics.AnalyticsAzureFunctions
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.Services.BuildServiceProvider()
                .GetService<IConfiguration>();
                _configuration = config;

            var sqlConnection = _configuration.GetConnectionString("analyticsdb_connection");

            builder.Services.AddDbContext<AnalyticsDbContext>(options =>
               options.UseSqlServer(sqlConnection));

            //Depedency Injection
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IBiobankWebService, BiobankWebService>();
            builder.Services.AddScoped<IGoogleAnalyticsReadService, GoogleAnalyticsReadService>();
            builder.Services.AddScoped<IAnalyticsReportGenerator, AnalyticsReportGenerator>();
            builder.Services.AddScoped<IGenericEFRepository<OrganisationAnalytic>, GenericEFRepository<OrganisationAnalytic>>();
            builder.Services.AddScoped<IGenericEFRepository<DirectoryAnalyticEvent>, GenericEFRepository<DirectoryAnalyticEvent>>();
            builder.Services.AddScoped<IGenericEFRepository<DirectoryAnalyticMetric>, GenericEFRepository<DirectoryAnalyticMetric>>();

        }

    }
}
