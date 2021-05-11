using Biobanks.Publications.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services.Contracts
{
    public interface IEpmcService
    {
        //Publications
        Task<PublicationDto> GetPublicationById(int publicationId);

        Task<List<PublicationDto>> GetOrganisationPublications(string biobank);

        //Annotations
        Task<List<AnnotationDto>> GetPublicationAnnotations(string publicationId, string source);
    }
}
