using Biobanks.Publications.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services.Contracts
{
    public interface IEpmcService
    {
        //Publications
        Task<PublicationDto> GetPublicationById(int publicationId);

        Task<List<PublicationDto>> GetOrganisationPublications(string biobank);

        //Annotations
        Task<List<AnnotationDTO>> GetPublicationAnnotations(string publicationId, string source);
    }
}
