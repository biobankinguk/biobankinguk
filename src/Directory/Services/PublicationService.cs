using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class PublicationService : IPublicationService
    {
        private readonly BiobanksDbContext _db;

        public PublicationService(BiobanksDbContext db)
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
                throw new KeyNotFoundException($"No Publication with Id=${publication} assigned to Organisation of Id=${organisationId}");

            publication.Accepted = accept;

            await _db.SaveChangesAsync();

            return publication;
        }
    }
}
