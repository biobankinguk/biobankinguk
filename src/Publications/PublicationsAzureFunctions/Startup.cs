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
   
        public override void Configure(IFunctionsHostBuilder builder)
        {

            string SqlConnection = "Server=tcp:publicationsazuresql.database.windows.net,1433;Initial Catalog=Publications;Persist Security Info=False;User ID=azureuser;Password=Publications123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            builder.Services.AddDbContext<PublicationDbContext>(options =>
                options.UseSqlServer(SqlConnection));
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IEPMCService, EMPCWebService>();
            builder.Services.AddScoped<IPublicationService, PublicationService>();

        }

    }
}
