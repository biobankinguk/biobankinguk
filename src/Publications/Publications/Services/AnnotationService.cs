using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Publications.Services.Dto;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Publications.Services
{
    public class AnnotationService : IAnnotationService
    {
        private BiobanksDbContext _ctx;

        public AnnotationService(BiobanksDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDTO> annotations)
        {
            var publication = await _ctx.Publications.Include(o => o.Annotations).FirstOrDefaultAsync(x => x.PublicationId == publicationId);

            var annotationList = new List<Annotation>();
            foreach (var annotation in annotations)
            {
                foreach (var tags in annotation.Tags)
                {
                    var annotationEntity = new Annotation(){Name = tags.Name.ToLower()};
                    annotationList.Add(annotationEntity);
                }
            }

            //Remove duplicate Annotation Names
            var annList = annotationList.GroupBy(x => x.Name).Select(x => x.First()).ToHashSet();
            
            foreach (var annotation in annList)
            {
                //If annotation doesn't already exist (new annotation)
                if (_ctx.Annotations.Any(x => x.Name == annotation.Name))
                {
                    //link annotation to publication
                    var oldAnnotation = _ctx.Annotations.First(x => x.Name == annotation.Name);
                    if (!publication.Annotations.Any(x => x.Id == oldAnnotation.Id))
                        publication.Annotations.Add(oldAnnotation);
                }
                else
                {
                    //add new annotation and link to publication
                    publication.Annotations.Add(annotation);
                }
            }

            publication.AnnotationsSynced = DateTime.Now;
            await _ctx.SaveChangesAsync();
        }
    }
}
