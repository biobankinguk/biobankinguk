using Microsoft.Extensions.Hosting;
using System.Threading;
using Biobanks.Publications.Services.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Biobanks.Publications.Services.Hosted
{
    public class FetchAnnotationsService : IHostedService
    {
        private readonly IAnnotationService _annotationService;
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEpmcService _epmcWebService;

        private readonly ILogger<FetchAnnotationsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public FetchAnnotationsService(IAnnotationService annotationService,
            IBiobankReadService biobankReadService,
            IEpmcService epmcWebService,
            ILogger<FetchAnnotationsService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _annotationService = annotationService;
            _biobankReadService = biobankReadService;
            _epmcWebService = epmcWebService;

            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Call directory for all active organisation
            var biobanks = (await _biobankReadService.ListBiobanksAsync()).Where(x => !x.ExcludePublications).ToList();
            _logger.LogInformation($"Fetching annotations for {biobanks.Count()} organisations");

            foreach (var biobank in biobanks)
            {
                //Fetch all publications for each organisation in DB
                var publications = await _biobankReadService.ListOrganisationPublications(biobank.OrganisationId);

                foreach (var publication in publications)
                {
                    //Fetch all annotations for each publication
                    var annotations = await _epmcWebService.GetPublicationAnnotations(publication.PublicationId, publication.Source);

                    await _annotationService.AddPublicationAnnotations(publication.PublicationId, annotations);

                    _logger.LogInformation($"Fetched {annotations.Count()} annotations for {publication}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
