using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Publications.Services.Contracts;
using Publications;
using Publications.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer;
using Directory.Data.Entities;

[assembly: FunctionsStartup(typeof(PublicationsAzureFunctions.Startup))]

namespace PublicationsAzureFunctions
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        
        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = builder.Services.BuildServiceProvider()
                .GetService<IConfiguration>();

            // Populate connection string with credentials
            var sqlConnection = _configuration.GetConnectionString("sqldb-connection");
            var sqlUsername = _configuration.GetValue("sqldb-username", "");
            var sqlPassword = _configuration.GetValue("sqldb-password", "");

            sqlConnection = String.Format(sqlConnection, sqlUsername, sqlPassword);

            // Register DbContext
            builder.Services.AddDbContext<PublicationDbContext>(options =>
               options.UseSqlServer(sqlConnection));

            // DI
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IEpmcService, EpmcWebService>();
            builder.Services.AddScoped<IPublicationService, PublicationService>();
            builder.Services.AddScoped<IBiobankReadService, BiobankReadService>();
            builder.Services.AddTransient<FetchPublicationsService>();
        }
    }
}
