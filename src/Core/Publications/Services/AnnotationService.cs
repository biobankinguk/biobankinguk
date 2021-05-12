using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Publications.Dto;

namespace Biobanks.Publications.Services
{
    public class AnnotationService : IAnnotationService
    {
        private BiobanksDbContext _db;

        public AnnotationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations)
        {
            var publication = await _db.Publications.Include(o => o.Annotations).FirstOrDefaultAsync(x => x.PublicationId == publicationId);

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
                if (_db.Annotations.Any(x => x.Name == annotation.Name))
                {
                    //link annotation to publication
                    var oldAnnotation = _db.Annotations.First(x => x.Name == annotation.Name);
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
            await _db.SaveChangesAsync();
        }
    }
}
