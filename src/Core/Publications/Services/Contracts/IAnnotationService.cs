using Biobanks.Publications.Dto;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services.Contracts
{
    public interface IAnnotationService
    {
        Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations);
    }
}
