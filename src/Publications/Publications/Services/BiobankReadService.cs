using Directory.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Publications.Entities;
using Publications.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        private PublicationDbContext _ctx;

        public BiobankReadService(
            PublicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IList<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _ctx.Organisations.Where(
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false)).ToListAsync();

        public async Task<IList<string>> GetOrganisationNames()
            => (await ListBiobanksAsync()).Select(x => x.Name).ToList();

        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _ctx.Publications.Where(x => x.OrganisationId == biobankId).ToListAsync();

        //Uses Publication Id from EF
        public async Task<IEnumerable<PublicationAnnotation>> GetPublicationAnnotations(int publicationId)
            => await _ctx.PublicationAnnotations.Where(x => x.Publication_Id == publicationId).Include(a => a.Annotation).ToListAsync();
        
        //Uses publicationId from API
        public async Task<Publication> GetPublicationById(string publicationId)
            => await _ctx.Publications.Where(x => x.PublicationId == publicationId).FirstOrDefaultAsync();

        //Uses Annotation Id from EF
        public async Task<Annotation> GetAnnotationById(int annotationId)
            => await _ctx.Annotations.Where(x => x.Id == annotationId).FirstOrDefaultAsync();

        public async Task<Annotation> GetAnnotationByName(string name)
            => await _ctx.Annotations.Where(x => x.Name == name).FirstOrDefaultAsync();

        public async Task<PublicationAnnotation> GetPublicationAnnotation(int publicationId, string uri)
        {
            var query =
                from PublicationAnnotations in _ctx.PublicationAnnotations
                join Annotations in _ctx.Annotations on new { Annotation_Id = PublicationAnnotations.Annotation_Id } equals new { Annotation_Id = Annotations.Id } into Annotations_join
                from Annotations in Annotations_join.DefaultIfEmpty()
                where
                  Annotations.Uri == uri &&
                  PublicationAnnotations.Publication_Id == publicationId
                select new PublicationAnnotation
                {
                    Publication_Id = PublicationAnnotations.Publication_Id,
                    Annotation_Id = PublicationAnnotations.Annotation_Id
                };

            return await query.FirstOrDefaultAsync();
        }


    }
}
