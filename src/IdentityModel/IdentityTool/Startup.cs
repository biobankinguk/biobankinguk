using Biobanks.Data;
using Biobanks.IdentityTool.Commands.Runners;
using Biobanks.IdentityTool.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.IdentityTool
{
    internal static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Enhance our IConfiguration with CommandLine values as specified
            services.AddEnhancedConfiguration(context, new()
            {
                ["--connection-string"] = "ConnectionStrings:Default"
            });

            // only add EF if we have a connection string to allow adding
            // Commands will need to validate the connection string / service availability
            // since not all Commands require it
            var connectionString = context.Configuration.GetConnectionString("Default");
            if (connectionString is not null)
                services.AddDbContext<BiobanksDbContext>(
                    o => o.UseSqlServer(connectionString));

            // Command Runners
            services
                .AddTransient<AddApiClient>()
                .AddTransient<GenerateId>()
                .AddTransient<Hash>();
        }
    }
}
