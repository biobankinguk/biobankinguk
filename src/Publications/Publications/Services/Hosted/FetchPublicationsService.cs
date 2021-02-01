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
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEpmcService _epmcWebService;

        private readonly ILogger<FetchPublicationsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FetchPublicationsService(IPublicationService publicationService,
            IBiobankReadService biobankReadService,
            IEpmcService epmcWebService,
            ILogger<FetchPublicationsService> logger, 
            IServiceScopeFactory scopeFactory)
        {
            _publicationService = publicationService;
            _biobankReadService = biobankReadService;
            _epmcWebService = epmcWebService;

            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Call directory for all active organisation
            var biobanks = (await _biobankReadService.ListBiobanksAsync()).Where(x => x.IncludePublications != false).ToList();

            _logger.LogInformation($"Fetching publications for {biobanks.Count()} organisations");

            // Fetch and store all publications for each organisation
            foreach (var biobank in biobanks)
            {
                var publications = await _epmcWebService.GetOrganisationPublications(biobank.Name);
                await _publicationService.AddOrganisationPublications(biobank.OrganisationId, publications);

                _logger.LogInformation($"Fetched {publications.Count()} publications for {biobank}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
