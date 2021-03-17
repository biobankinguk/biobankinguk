using IdentityTool.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityTool
{
    internal static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            services.AddTransient<GenerateSecretHandler>();
        }
    }
}
