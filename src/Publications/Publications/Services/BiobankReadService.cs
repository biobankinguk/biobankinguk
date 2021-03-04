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
        
        //Gets Publications by OrganisationId, if Accepted is true, if the pub has no previously synced annotations and if last sync was over a month ago
        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _ctx.Publications.Where(x => x.OrganisationId == biobankId && x.Accepted == true)
            .Where(a => a.AnnotationsSynced == null || a.AnnotationsSynced < DateTime.Today.AddMonths(-1)).ToListAsync();

        //Uses Publication Id from EF
        public async Task<IEnumerable<PublicationAnnotation>> GetPublicationAnnotations(int publicationId)
            => await _ctx.PublicationAnnotations.Where(x => x.PublicationsId == publicationId).Include(a => a.Annotation).ToListAsync();
        
        //Uses publicationId from API
        public async Task<Publication> GetPublicationById(string publicationId)
            => await _ctx.Publications.Where(x => x.PublicationId == publicationId).FirstOrDefaultAsync();

        //Uses Annotation Id from EF
        public async Task<Annotation> GetAnnotationById(int annotationId)
            => await _ctx.Annotations.Where(x => x.Id == annotationId).FirstOrDefaultAsync();

        public async Task<Annotation> GetAnnotationByName(string name)
            => await _ctx.Annotations.Where(x => x.Name == name).Include(a => a.PublicationAnnotations).FirstOrDefaultAsync();

    }
}
