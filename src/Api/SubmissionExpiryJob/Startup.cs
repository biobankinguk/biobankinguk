using Biobanks.Common.Data;
using Biobanks.SubmissionExpiryJob.Services;
using Biobanks.SubmissionExpiryJob.Services.Contracts;
using Biobanks.SubmissionExpiryJob.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Biobanks.SubmissionExpiryJob.Startup))]

namespace Biobanks.SubmissionExpiryJob
{
    class Startup : FunctionsStartup
    {
        private IConfiguration _configuration;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            _configuration = builder.Services.BuildServiceProvider()
                .GetService<IConfiguration>();

            // Settings From Configuration
            builder.Services.Configure<SettingsModel>(options =>
            {
                options.ExpiryDays = _configuration.GetValue<int>("expiryDays", 0);
            });

            // Register DbContext
            builder.Services.AddDbContext<SubmissionsDbContext>(options =>
                    options.UseSqlServer(
                        _configuration.GetConnectionString("sqldb-connection")
                    )
                );

            // DI
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();
        }
    }
}