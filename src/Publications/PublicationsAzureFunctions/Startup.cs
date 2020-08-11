using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Publications.Services.Contracts;
using Publications;
using Publications.Services;

[assembly: FunctionsStartup(typeof(PublicationsAzureFunctions.Startup))]

namespace PublicationsAzureFunctions
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            _configuration = config;

            var SqlConnection = Environment.GetEnvironmentVariable("sqldb_connection");
            builder.Services.AddDbContext<PublicationDbContext>(options =>
                options.UseSqlServer(SqlConnection));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IEPMCService, EMPCWebService>();
            builder.Services.AddScoped<IPublicationService, PublicationService>();
            builder.Services.AddScoped<IBiobankService, BiobankWebService>();

        }

    }
}
