using Biobanks.Publications.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services.Contracts
{
    public interface IAnnotationService
    {
        Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations);
    }
}
