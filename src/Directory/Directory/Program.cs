using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Directory
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(b => b
                    .AddJsonFile("appsettings.identity.json", optional: false)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args))
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder.UseStartup<Startup>());
    }
}
