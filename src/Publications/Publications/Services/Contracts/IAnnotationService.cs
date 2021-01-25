using Publications.Entities;
using Publications.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IAnnotationService
    {
        Task AddPublicationAnnotations(int publicationId, AnnotationResult annotations);
    }
}
