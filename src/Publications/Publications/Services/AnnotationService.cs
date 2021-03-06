﻿using System;
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


        public async Task AddPublicationAnnotations(string publicationId, IEnumerable<AnnotationDTO> annotations)
        {

            var publication = await _biobankReadService.GetPublicationById(publicationId);
            var existingAnnotations = await _biobankReadService.GetPublicationAnnotations(publication.Id);

            var annotationList = new List<Annotation>();
            foreach(var annotation in annotations)
            {
                foreach(var tags in annotation.Tags)
                {
                      var annotationEntity = new Annotation()
                      {
                          Name = tags.Name.ToLower(),
                          PublicationAnnotations = new List<PublicationAnnotation>()
                      };
                      var publicationAnnotation = new PublicationAnnotation()
                      {
                          Annotation_Id = annotationEntity.Id,
                          Publication_Id = publication.Id
                      };
                      annotationEntity.PublicationAnnotations.Add(publicationAnnotation);
                      annotationList.Add(annotationEntity);

                }
            }
            //Remove duplicate Annotation Names
            var annList = annotationList.GroupBy(x => x.Name).Select(x => x.First()).ToHashSet();
   
            //Add or Update new annotations
            foreach (var newer in annList)
            {
                //Find if older version of annotation exists
                var older = await _biobankReadService.GetAnnotationByName(newer.Name);


                if (older is null)
                {
                   //Add new record
                   _ctx.Add(newer);
                }
                else
                {
                    //Check if publicationAnnotation already exists 
                    var publicationAnnotation = older.PublicationAnnotations.FirstOrDefault(x => x.Publication_Id == publication.Id);

                    if (publicationAnnotation is null)
                    {
                        older.PublicationAnnotations.Add(new PublicationAnnotation()
                        {
                            Annotation_Id = older.Id,
                            Publication_Id = publication.Id
                        });

                        _ctx.Update(older);
                    }
                }
            }
      
            publication.AnnotationsSynced = DateTime.Now;
            _ctx.Update(publication);

           await _ctx.SaveChangesAsync();
        }
    }
}
