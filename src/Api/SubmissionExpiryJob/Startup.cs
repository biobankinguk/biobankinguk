using Biobanks.Common.Data;
using Biobanks.SubmissionExpiryJob.Services;
using Biobanks.SubmissionExpiryJob.Services.Contracts;
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

            var sqlConnection = _configuration.GetConnectionString("sqldb-connection");

            // Register DbContext
            builder.Services.AddDbContext<SubmissionsDbContext>(options =>
               options.UseSqlServer(sqlConnection));

            // DI
            builder.Services.AddScoped<ISubmissionService, SubmissionService>();
        }
    }
}