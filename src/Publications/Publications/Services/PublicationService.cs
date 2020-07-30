using Microsoft.EntityFrameworkCore;
using Publications.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services
{
    public class PublicationService : IPublicationService
    {

        private PublicationDbContext _ctx;

        public PublicationService(PublicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddOrganisationPublications(string organisationName, IEnumerable<PublicationDTO> publications)
        {
            var pubs = publications.Select(x => new Publication()
            {
                PublicationId = x.Id,
                Organisation = organisationName,
                Title = x.Title,
                Authors = x.Authors,
                Journal = x.Journal,
                Year = x.Year,
                DOI = x.Doi
            });

            await _ctx.Publications.AddRangeAsync(pubs);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IEnumerable<PublicationDTO>> GetOrganisationPublications(string organisationName)
        {
            return await _ctx.Publications
                .Where(x => x.Organisation.Equals(organisationName))
                .Select(x => new PublicationDTO()
                {
                    Id = x.PublicationId,
                    Title = x.Title,
                    Authors = x.Authors,
                    Journal = x.Journal,
                    Year = x.Year,
                    Doi = x.DOI
                })
                .ToListAsync();
        }
    }
}
