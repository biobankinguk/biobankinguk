using Biobanks.Publications.Services.Contracts;
using Biobanks.Shared.Services.Contracts;

using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class PublicationsJob
    {

        private readonly IPublicationJobService _publicationService;
        private readonly IAnnotationService _annotationService;
        private readonly IEpmcService _epmcWebService;
        private readonly IOrganisationService _organisationService;
        
        public PublicationsJob(
            IPublicationJobService publicationService,
            IAnnotationService annotationService,
            IEpmcService epmcWebService,
            IOrganisationService organisationService)
        {
            _publicationService = publicationService;
            _annotationService = annotationService;
            _epmcWebService = epmcWebService;
            _organisationService = organisationService;
        }

        public async Task Run()
        {
            var biobanks = await _organisationService.List();
            
            foreach (var biobank in biobanks.Where(x => !x.ExcludePublications))
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
