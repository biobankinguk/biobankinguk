using System;
using Biobanks.Data;
using Biobanks.DataSeed.Services;
using Biobanks.Services;
using Biobanks.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.DataSeed
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configure Dependencies
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var connString = context.Configuration["RefDataConnectionString"];

            if (string.IsNullOrWhiteSpace(connString))
            {
                Console.WriteLine("Couldn't find a connection string.");
                Console.WriteLine("Provide a value for RefDataConnectionString on the command line");
                Console.WriteLine("or in an appsettings.json,");
                Console.WriteLine("or set the environment variable 'DOTNET_RefDataConnectionString'.");

                throw new Exception("RefDataConnectionString not set.");
            }

            Console.WriteLine("Using the following connection string:");
            Console.WriteLine(connString);
            Console.WriteLine();
            Console.WriteLine("Press any key to continue with this connection string; otherwise close the application");
            Console.ReadKey();

            services.AddDbContext<BiobanksDbContext>(opts =>
                opts.UseSqlServer(connString)
            );

            services.AddHttpClient();
        }

        /// <summary>
        /// Configure Startup Services
        /// </summary>
        public static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddScoped<IBiobankReadService, BiobankReadService>();
            services.AddSingleton<CountriesWebService>();
            services.AddHostedService<SeedingService>();
        }
    }
}
