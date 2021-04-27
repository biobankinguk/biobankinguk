using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Publications.Services.Contracts;
using Biobanks.Publications.Services.Dto;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Data;

namespace Biobanks.Publications.Services
{
    public class AnnotationService : IAnnotationService
    {
        private BiobanksDbContext _ctx;
        private IBiobankReadService _biobankReadService;

        public AnnotationService(BiobanksDbContext ctx, IBiobankReadService biobankReadService)
        {
            _ctx = ctx;
            _biobankReadService = biobankReadService;
        }


        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDTO> annotations)
        {

            var publication = await _biobankReadService.GetPublicationById(publicationId);

            foreach (var annotation in annotations)
            {
                foreach (var tags in annotation.Tags)
                {
                    //If annotation doesn't already exist (new annotation)
                    if (!_ctx.Annotations.Any(x => x.Name == tags.Name.ToLower()))
                    {
                        //add new annotation and link to publication
                        publication.Annotations.Add(new Annotation { Name = tags.Name.ToLower() });
                    }
                    else
                    {
                        //link annotation to publication
                        var oldAnnotation = _ctx.Annotations.First(x => x.Name == tags.Name.ToLower());
                        if (publication.Annotations.Any(x=>x.Id == oldAnnotation.Id))
                            publication.Annotations.Add(oldAnnotation);
                    }
                }
            }
      
            publication.AnnotationsSynced = DateTime.Now;
            _ctx.Update(publication);

           await _ctx.SaveChangesAsync();
        }
    }
}
