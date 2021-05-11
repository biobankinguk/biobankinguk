using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using UoN.VersionInformation;

namespace Biobanks.DataSeed
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {          
            if (args.Length > 0 && (args[0].Equals("-v") || args[0].Equals("--version")))
            {
                VersionInformationService version = new VersionInformationService();                
                Console.WriteLine(version.EntryAssembly());
            }
            else
            {
                await CreateHostBuilder(args).RunConsoleAsync();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices(Startup.ConfigureServices)
                .ConfigureServices(Startup.Configure);

    }
}

