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
        private readonly IPublicationService _publicationService;
        private readonly IBiobankService _biobankWebService;
        private readonly IEpmcService _epmcWebService;

        private readonly ILogger<FetchPublicationsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FetchPublicationsService(IPublicationService publicationService,
            IBiobankService biobankWebService,
            IEpmcService epmcWebService,
            ILogger<FetchPublicationsService> logger, 
            IServiceScopeFactory scopeFactory)
        {
            _publicationService = publicationService;
            _biobankWebService = biobankWebService;
            _epmcWebService = epmcWebService;

            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Call directory for all active organisation
            var biobanks = await _biobankWebService.GetOrganisationNames();

            _logger.LogInformation($"Fetching publications for {biobanks.Count()} organisations");

            // Fetch and store all publications for each organisation
            foreach (var biobank in biobanks)
            {
                var publications = await _epmcWebService.GetOrganisationPublications(biobank);
                await _publicationService.AddOrganisationPublications(biobank, publications);

                _logger.LogInformation($"Fetched {publications.Count()} publications for {biobank}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
