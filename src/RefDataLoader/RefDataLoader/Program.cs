using Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public class Program
    {
        public static IConfiguration Configuration;

        //todo figure out how to use fancy core 3.0 stuff to tidy this up
        static async Task Main()
        {
            var dataService = BuildDi()
               .GetService(typeof(IDataService)) as DataService;
            await dataService.SeedData();
        }

        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            // build config
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build();

            // make it available via DI
            services.AddSingleton(x => Configuration);

            //Settings
            services.Configure<ApiSettings>(
               Configuration.GetSection("ApiSettings"));
            services.AddTransient<IDataService, DataService>();


            return services.BuildServiceProvider();
        }

    }
}
