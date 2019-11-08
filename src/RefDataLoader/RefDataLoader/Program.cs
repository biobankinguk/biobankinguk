using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace RefDataLoader
{
    public static class Program
    {
        static async Task Main(string[] args)
            => await CreateHostBuilder(args).RunConsoleAsync();

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(Startup.ConfigureServices)
                .ConfigureServices(Startup.Configure);
    }
}
