using IdentityTool.Commands.Runners;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityTool
{
    internal static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            // Command Runners
            services.AddTransient<GenerateSecret>();
        }
    }
}
