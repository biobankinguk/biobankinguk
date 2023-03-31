using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class PublicationService : IPublicationService
    {
        private readonly ApplicationDbContext _db;

        public PublicationService(ApplicationDbContext db)
        {
            _db = db;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<Publication>> ListByOrganisation(int organisationId, bool acceptedOnly = false)
            => await _db.Publications
                .AsNoTracking()
                .Where(x => x.OrganisationId == organisationId)
                .Where(x => !acceptedOnly || (x.Accepted ?? false)) // Logical Implication
                .ToListAsync();

        ///<inheritdoc/>
        public async Task<Publication> Create(Publication publication)
        {
            _db.Publications.Add(publication);
            await _db.SaveChangesAsync();
             
            return publication;
        }

        ///<inheritdoc/>
        public async Task<Publication> Claim(string publicationId, int organisationId, bool accept = true)
        {
            var publication = await _db.Publications
                .FirstOrDefaultAsync(x => x.PublicationId == publicationId && x.OrganisationId == organisationId);

            if (publication is null)
                return null;

            publication.Accepted = accept;

            await _db.SaveChangesAsync();

            return publication;
        }
    }
}
