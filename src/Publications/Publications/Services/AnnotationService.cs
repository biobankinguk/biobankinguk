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
        private IBiobankReadService _biobankReadService;

        public AnnotationService(PublicationDbContext ctx, IBiobankReadService biobankReadService)
        {
            _ctx = ctx;
            _biobankReadService = biobankReadService;
        }


        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations)
        {

            var existingPublications = await _biobankReadService.GetPublicationById(publicationId);
            var existingAnnotations = await _biobankReadService.GetPublicationAnnotations(existingPublications.Id);

            var exAnnotations = new List<Annotation>();
            foreach (var annotationObj in existingAnnotations)
            {
                var annotation = await _biobankReadService.GetAnnotationById(annotationObj.Annotation_Id);
                exAnnotations.Add(annotation);
            }

            var annotationList = new List<Annotation>();

            foreach(var annotation in annotations)
            {
                foreach(var tags in annotation.Tags)
                {
                    var annotationEntity = new Annotation()
                    {
                        AnnotationId = annotation.Id,
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
                var older = exAnnotations.Where(x => x.AnnotationId == newer.AnnotationId).FirstOrDefault();
                if (older is null)
                {
                   //Add new record
                   _ctx.Add(newer);
                }
                else
                {
                   //Get annotation using annotation EF Id
                   var olderAnnotation = await _biobankReadService.GetAnnotationById(older.Id);
                   olderAnnotation.Name = newer.Name;
                   olderAnnotation.Uri = newer.Uri;
                   _ctx.Update(olderAnnotation);
                }
            }
           await _ctx.SaveChangesAsync();
        }
    }
}
