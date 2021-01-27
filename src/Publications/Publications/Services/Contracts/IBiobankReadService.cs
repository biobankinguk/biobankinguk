using Directory.Data.Entities;
using Publications.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IList<string>> GetOrganisationNames();
        Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
        Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId);

        Task<IEnumerable<PublicationAnnotation>> GetPublicationAnnotations(int publicationId);
        Task<Publication> GetPublicationById(string publicationId);

        Task<Annotation> GetAnnotationById(int annotationId);
    }
}