using System;
using System.Net.Http.Headers;
using Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace RefDataLoader
{
    public static class Startup
    {
        /// <summary>
        /// Configure Dependencies
        /// </summary>
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var config = context.Configuration;

            services.Configure<ApiSettings>(config.GetSection("ApiSettings"));

            services.AddHttpClient(string.Empty, client =>
            {
                client.BaseAddress = new Uri(config["ApiSettings:BaseUri"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }

        /// <summary>
        /// Configure Startup Services
        /// </summary>
        public static void Configure(HostBuilderContext context, IServiceCollection services)
        {
            services.AddHostedService<DataService>();
        }
    }
}
