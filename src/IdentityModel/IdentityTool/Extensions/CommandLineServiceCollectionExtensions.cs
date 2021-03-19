using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System.Collections.Generic;
using System.CommandLine.Invocation;

namespace Biobanks.IdentityTool.Extensions
{
    internal static class CommandLineServiceCollectionExtensions
    {
        /// <summary>
        /// Enhance the IConfiguration with Option Values from the CommandLine InvocationContext
        /// </summary>
        /// <param name="context"></param>
        /// <param name="configurationMap">A dictionary that maps CommandLine Option alias Keys to IConfiguration Key Values</param>
        public static IServiceCollection AddEnhancedConfiguration(
            this IServiceCollection services,
            HostBuilderContext context,
            Dictionary<string, string> configurationMap)
        {
            // Get the CommandLine invocation context so we can get argument values
            var invocationContext = (InvocationContext)context.Properties[typeof(InvocationContext)];

            var configValues = new List<KeyValuePair<string, string>>();

            foreach (var key in configurationMap.Keys)
            {
                // try and get the key from the commandline // TODO: can we get it as a string even if it isn't?
                var cliValue = invocationContext.ParseResult.ValueForOption<string>("-c");
                if (cliValue is not null)
                    configValues.Add(new(configurationMap[key], cliValue));
            }

            if (configValues.Count == 0) return services;

            context.Configuration = new ConfigurationBuilder()
                .AddConfiguration(context.Configuration)
                .AddInMemoryCollection(configValues)
                .Build();

            return services.AddSingleton(context.Configuration);
        }
    }
}
