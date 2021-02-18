using Publications.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
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
