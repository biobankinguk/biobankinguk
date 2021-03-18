using Biobanks.IdentityModel.Services;

using Biobanks.IdentityTool.Commands.Runners;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Biobanks.IdentityTool
{
    internal static class Startup
    {
        public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {


            services
                .AddTransient<CryptoService>()

                // Command Runners
                .AddTransient<GenerateId>();
        }
    }
}
