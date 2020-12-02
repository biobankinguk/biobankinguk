using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Analytics.Services.Contracts;
using Analytics.Services;
using Analytics.Data;
using Analytics.Data.Entities;
using System;

[assembly: FunctionsStartup(typeof(Analytics.AnalyticsAzureFunctions.Startup))]

namespace Analytics.AnalyticsAzureFunctions
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = builder.Services.BuildServiceProvider()
                .GetService<IConfiguration>();

            // Populate connection string with credentials
            var sqlConnection = _configuration.GetConnectionString("analyticsdb_connection");
            var sqlUsername = _configuration.GetValue("sqldb-username", "");
            var sqlPassword = _configuration.GetValue("sqldb-password", "");

            sqlConnection = String.Format(sqlConnection, sqlUsername, sqlPassword);

            builder.Services.AddDbContext<AnalyticsDbContext>(options =>
               options.UseSqlServer(sqlConnection, options => options.EnableRetryOnFailure()));

            //Depedency Injection
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IAnalyticsReportGenerator, AnalyticsReportGenerator>();
            builder.Services.AddTransient<IBiobankWebService, BiobankWebService>();
            builder.Services.AddTransient<IGoogleAnalyticsReadService, GoogleAnalyticsReadService>();
        }

    }
}
