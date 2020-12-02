using Microsoft.Azure.Functions.Extensions.DependencyInjection;
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

            // Populate connection string with credentials
            //var sqlConnection = _configuration.GetConnectionString("sqldb-connection");
            //var sqlUsername = _configuration.GetValue("sqldb-username", "");
            //var sqlPassword = _configuration.GetValue("sqldb-password", "");

            //sqlConnection = String.Format(sqlConnection, sqlUsername, sqlPassword);

            //// Register DbContext
            //builder.Services.AddDbContext<PublicationDbContext>(options =>
            //   options.UseSqlServer(sqlConnection));

            //// DI
            //builder.Services.AddHttpClient();
            //builder.Services.AddScoped<IEpmcService, EpmcWebService>();
            //builder.Services.AddScoped<IPublicationService, PublicationService>();
            //builder.Services.AddScoped<IBiobankReadService, BiobankReadService>();
            //builder.Services.AddTransient<FetchPublicationsService>();
        }
    }
}