using Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public static class Program
    {
        //todo figure out how to use fancy core 3.0 stuff to tidy this up
        static async Task Main()
        {
            var dataService = (DataService)ConfigureServices()
               .GetService(typeof(IDataService));
            await dataService.SeedData();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Config
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables()
                .Build();
            services.AddSingleton(_ => config);
            services.Configure<ApiSettings>(config.GetSection("ApiSettings"));

            // Services
            services.AddHttpClient<IDataService, DataService>(client =>
            {
                client.BaseAddress = new Uri(config["ApiSettings:BaseUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddTransient<IDataService, DataService>();

            return services.BuildServiceProvider();
        }
    }
}
