using Biobanks.Publications.Services.Contracts;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Biobanks.Publications.Services.Hosted
{
    public class FetchAnnotationsService
    {
        private readonly IAnnotationService _annotationService;
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEpmcService _epmcWebService;

        private readonly ILogger<FetchAnnotationsService> _logger;

        public FetchAnnotationsService(IAnnotationService annotationService,
            IBiobankReadService biobankReadService,
            IEpmcService epmcWebService,
            ILogger<FetchAnnotationsService> logger)
        {
            _annotationService = annotationService;
            _biobankReadService = biobankReadService;
            _epmcWebService = epmcWebService;

            _logger = logger;
        }
        public async Task StartAsync()
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

                    _logger.LogInformation($"Fetched {annotations.Count()} annotations for {publication.PublicationId}");
                }
            }
        }

        public Task StopAsync() => Task.CompletedTask;
    }
}
