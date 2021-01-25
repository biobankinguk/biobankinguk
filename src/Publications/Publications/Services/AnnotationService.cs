using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Publications.Services.Contracts;
using Publications.Services.Dto;
using Publications.Entities;

namespace Publications.Services
{
    public class AnnotationService : IAnnotationService
    {
        private PublicationDbContext _ctx;

        public AnnotationService(PublicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task AddPublicationAnnotations(int publicationId, AnnotationResult annotations)
        {
            var existingAnnotations = _ctx.PublicationAnnotations.Where(x => x.PublicationId == publicationId);

            //Need to drill down to Tag Level to get name and uri
            var fetchedAnnotations = annotations.Annotations.Select(x => new Annotation()
            {

            });

   
            throw new NotImplementedException();
        }


    }
}
