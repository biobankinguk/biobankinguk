using Publications.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IAnnotationService
    {
        Task AddPublicationAnnotations(int publicationId, IEnumerable<AnnotationDto> annotations);
    }
}
