using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Biobanks.SubmissionApi
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
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Scaffolds the base webhost to be configured.
        /// </summary>
        /// <param name="args">Command-line arguments - not currently used.</param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>()
                .ConfigureKestrel((context, opts) =>
                {
                    opts.AllowSynchronousIO = true;
                });
    }
}
