using Biobanks.Publications.Core.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class PublicationsJob
    {

        private readonly IPublicationService _publicationService;
        private readonly IAnnotationService _annotationService;
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEpmcService _epmcWebService;

        public PublicationsJob(
            IPublicationService publicationService,
            IAnnotationService annotationService,
            IBiobankReadService biobankReadService,
            IEpmcService epmcWebService)
        {
            _publicationService = publicationService;
            _annotationService = annotationService;
            _biobankReadService = biobankReadService;
            _epmcWebService = epmcWebService;
        }

        public async Task Run()
        {
            var biobanks = (await _biobankReadService.ListBiobanksAsync()).Where(x => !x.ExcludePublications).ToList();

            foreach (var biobank in biobanks)
            {
                // Update Biobanks Publication Collection
                await _publicationService.AddOrganisationPublications(biobank.OrganisationId,
                    await _epmcWebService.GetOrganisationPublications(biobank.Name));

                // Update Publications Annotations
                foreach (var publication in await _publicationService.ListOrganisationPublications(biobank.OrganisationId))
                {
                    await _annotationService.AddPublicationAnnotations(publication.PublicationId, 
                        await _epmcWebService.GetPublicationAnnotations(publication.PublicationId, publication.Source));
                }
            }
        }
    }
}
