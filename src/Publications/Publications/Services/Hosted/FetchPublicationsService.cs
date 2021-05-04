using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Biobanks.Publications.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services.Hosted
{
    public class FetchPublicationsService
    {
        private readonly IPublicationService _publicationService;
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEpmcService _epmcWebService;

        private readonly ILogger<FetchPublicationsService> _logger;

        public FetchPublicationsService(IPublicationService publicationService,
            IBiobankReadService biobankReadService,
            IEpmcService epmcWebService,
            ILogger<FetchPublicationsService> logger)
        {
            _publicationService = publicationService;
            _biobankReadService = biobankReadService;
            _epmcWebService = epmcWebService;

            _logger = logger;
        }

        public async Task StartAsync()
        {
            // Call directory for all active organisation
            var biobanks = (await _biobankReadService.ListBiobanksAsync()).Where(x => !x.ExcludePublications).ToList();

            _logger.LogInformation($"Fetching publications for {biobanks.Count()} organisations");

            // Fetch and store all publications for each organisation
            foreach (var biobank in biobanks)
            {
                var publications = await _epmcWebService.GetOrganisationPublications(biobank.Name);
                await _publicationService.AddOrganisationPublications(biobank.OrganisationId, publications);

                _logger.LogInformation($"Fetched {publications.Count()} publications for {biobank.OrganisationExternalId}");
            }
        }

        public Task StopAsync() => Task.CompletedTask;
    }
}
