using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Publications.Services.Contracts;
using Publications;

[assembly: FunctionsStartup(typeof(PublicationsAzureFunctions.Startup))]

namespace PublicationsAzureFunctions
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddScoped<IEPMCService, EMPCWebService>();

        }
    }
}
