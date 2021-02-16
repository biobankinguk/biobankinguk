using Directory.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Publications.Entities;
using Publications.Services.Contracts;
using Publications.Services.Dto;
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

        public async Task<IList<int>> GetOrganisationIds()
            => (await ListBiobanksAsync()).Select(x => x.OrganisationId).ToList();

        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _ctx.Publications.Where(x => x.OrganisationId == biobankId && x.Accepted == true)
            .Where(a => a.AnnotationsSynced == null || a.AnnotationsSynced < DateTime.Today.AddMonths(-1)).ToListAsync();

        public async Task<IEnumerable<Publication>> ListPublications(string publicationId, string source)
            => await _ctx.Publications.Where(x => x.PublicationId != publicationId && x.Source != source).Include(a => a.PublicationAnnotations).ThenInclude(b => b.Annotation).ToListAsync();

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
            => await _ctx.Annotations.Where(x => x.Name == name).Include(a => a.PublicationAnnotations).FirstOrDefaultAsync();


        //Gets all Annotations for every biobank in db
        public async Task<IEnumerable<AnnotationQueryDTO>> GetBiobankAnnotations()
        {
            var query =
                from Annotations in _ctx.Annotations
                join PublicationAnnotations in _ctx.PublicationAnnotations on new { Id = Annotations.Id } equals new { Id = PublicationAnnotations.Annotation_Id } into PublicationAnnotations_join
                from PublicationAnnotations in PublicationAnnotations_join.DefaultIfEmpty()
                join Publications in _ctx.Publications on new { Publication_Id = PublicationAnnotations.Publication_Id } equals new { Publication_Id = Publications.Id } into Publications_join
                from Publications in Publications_join.DefaultIfEmpty()
                orderby
                  Publications.OrganisationId
                select new AnnotationQueryDTO
                {
                    Annotation = Annotations.Name,
                    OrganisationId = Publications.OrganisationId
                };
            var annotationList = await query.ToListAsync();

            var result = annotationList.GroupBy(i => i.OrganisationId).Select(x => new AnnotationQueryDTO { OrganisationId = x.Key, Annotations = x.Select(e => e.Annotation).ToList() });

            return result;
        }
    }
}
