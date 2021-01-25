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


        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations)
        {
            var existingAnnotations = _ctx.PublicationAnnotations.Where(x => x.Publication_Id == int.Parse(publicationId));
            var existingPublications = _ctx.Publications.Where(x => x.PublicationId == publicationId).FirstOrDefault();
            var annotationList = new List<Annotation>();

            foreach(var annotation in annotations)
            {
                foreach(var tags in annotation.Tags)
                {
                    var annotationEntity = new Annotation()
                    {
                        Name = tags.Name,
                        Uri = tags.Uri,
                        PublicationAnnotations = new List<PublicationAnnotation>()
                    };
                    var publicationAnnotation = new PublicationAnnotation()
                    {
                        Annotation_Id = annotationEntity.Id,
                        Publication_Id = existingPublications.Id
                    };


                    annotationEntity.PublicationAnnotations.Add(publicationAnnotation);
                    annotationList.Add(annotationEntity);
                }
            }

            foreach (var newer in annotationList)
            {
                foreach (var test in newer.PublicationAnnotations)
                {
                    var older = existingAnnotations.Where(x => x.Annotation_Id == test.Annotation_Id).FirstOrDefault();
                    if (older is null)
                    {
                        _ctx.Add(newer);
                    }
                    else
                    {
                        
                    }
                }

            }

            await _ctx.SaveChangesAsync();
        }


    }
}
