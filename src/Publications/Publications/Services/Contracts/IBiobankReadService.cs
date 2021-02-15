using Directory.Data.Entities;
using Publications.Entities;
using Publications.Services.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
        Task<IList<int>> GetOrganisationIds();
        Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId);

        Task<IEnumerable<PublicationAnnotation>> GetPublicationAnnotations(int publicationId);
        Task<Publication> GetPublicationById(string publicationId);

        Task<Annotation> GetAnnotationById(int annotationId);

        Task<Annotation> GetAnnotationByName(string name);

        Task<IEnumerable<Publication>> ListPublications();

        Task<IEnumerable<AnnotationQueryDTO>> GetBiobankAnnotations();

    }
}