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
        private ApplicationDbContext _db;

        public AnnotationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDto> annotations)
        {
            var publication = await _db.Publications.Include(o => o.Annotations).FirstOrDefaultAsync(x => x.PublicationId == publicationId);

            var annotationList = annotations
                    .SelectMany(x => x.Tags)
                    .Select(x => new Annotation
                    {
                        Value = x.Name.ToLower()
                    })
                    .ToList();

            //Remove duplicate Annotation Names
            var annList = annotationList.GroupBy(x => x.Value).Select(x => x.First()).ToHashSet();
            
            foreach (var annotation in annList)
            {
                //If annotation doesn't already exist (new annotation)
                if (_db.Annotations.Any(x => x.Value == annotation.Value))
                {
                    //link annotation to publication
                    var oldAnnotation = _db.Annotations.First(x => x.Value == annotation.Value);
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
