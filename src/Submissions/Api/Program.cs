using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Biobanks.Submissions.Api
{
    /// <summary>
    /// Main Program init for app.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for app.
        /// </summary>
        /// <param name="args">Command-line arguments - not currently used.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Scaffolds the base webhost to be configured.
        /// </summary>
        /// <param name="args">Command-line arguments - not currently used.</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureServices(services => services.AddAutofac());
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, opts) =>
                    {
                        opts.AllowSynchronousIO = true;
                    });
                });
    }
}
