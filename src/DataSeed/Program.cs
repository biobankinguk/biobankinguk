using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Biobanks.DataSeed
{
    internal static class Program
    {
        private static async Task Main(string[] args)
             => await CreateHostBuilder(args).RunConsoleAsync();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(Startup.ConfigureServices)
                .ConfigureServices(Startup.Configure);

    }
}

