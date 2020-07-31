using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Publications;
using Publications.Services;
using Publications.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class FetchPublicationsService : IHostedService
    {

        private readonly ILogger<FetchPublicationsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FetchPublicationsService(ILogger<FetchPublicationsService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                // DI of required services
                var publicationService = scope.ServiceProvider.GetRequiredService<IPublicationService>();
                var biobankWebService = scope.ServiceProvider.GetRequiredService<IBiobankService>();
                var epmcWebService = scope.ServiceProvider.GetRequiredService<IEMPCService>();

                // Call directory for all active organisation
                var biobanks = await biobankWebService.GetOrganisationNames();

                _logger.LogInformation($"Fetching publications for {biobanks.Count()} organisations");

                // Fetch and store all publications for each organisation
                foreach (var biobank in biobanks)
                {
                    var publications = await epmcWebService.GetOrganisationPublications(biobank);
                    await publicationService.AddOrganisationPublications(biobank, publications);

                    _logger.LogInformation($"Fetched {publications.Count()} publications for {biobank}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
