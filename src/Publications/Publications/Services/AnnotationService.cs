using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Publications.Services.Contracts;
using Publications.Services.Dto;

namespace Publications.Services
{
    public class AnnotationService : IAnnotationService
    {
        private PublicationDbContext _ctx;

        public AnnotationService(PublicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task AddPublicationAnnotations(int publicationId, IEnumerable<AnnotationDto> annotations)
        {
            
        }
    }
}
