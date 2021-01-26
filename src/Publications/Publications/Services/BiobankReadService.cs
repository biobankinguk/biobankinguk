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

        //This uses Publication Id from EF
        public async Task<IEnumerable<PublicationAnnotation>> GetPublicationAnnotations(int publicationId)
            => await _ctx.PublicationAnnotations.Where(x => x.Publication_Id == publicationId).ToListAsync();

        //This uses publicationId from publication itself not EF 
        public async Task<Publication> GetPublicationById(string publicationId)
            => await _ctx.Publications.Where(x => x.PublicationId == publicationId).FirstOrDefaultAsync();

        //This uses Annotation Id from EF
        public async Task<Annotation> GetAnnotationById(int annotationId)
            => await _ctx.Annotations.Where(x => x.Id == annotationId).FirstOrDefaultAsync();
    }
}
