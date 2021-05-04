using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Publications.Services.Dto;
using Biobanks.Entities.Data;
using Biobanks.Data;
using Biobanks.Publications.Services.Contracts;

namespace Biobanks.Publications.Services
{
    public class PublicationService : IPublicationService
    {

        private BiobanksDbContext _ctx;

        public PublicationService(BiobanksDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications)
        {
            var existingPublications = _ctx.Publications.Where(x => x.OrganisationId == organisationId);

            var fetchedPublications = publications.Select(x => new Publication()
            {
                PublicationId = x.Id,
                OrganisationId = organisationId,
                Title = x.Title,
                Authors = x.Authors,
                Journal = x.Journal,
                Year = x.Year,
                DOI = x.Doi,
                Source = x.Source
            });

            // Add or Update new publications
            foreach (var newer in fetchedPublications)
            {
                // Find if older verison of publication exists
                var older = existingPublications.Where(x => x.PublicationId == newer.PublicationId).FirstOrDefault();

                if (older is null)
                {
                    // Add new Record
                    _ctx.Add(newer);
                }
                else
                {
                    // Update existing record
                    older.Title = newer.Title;
                    older.Authors = newer.Authors;
                    older.Journal = newer.Journal;
                    older.Year = newer.Year;
                    older.DOI = newer.DOI;
                    older.Source = newer.Source;

                    _ctx.Update(older);
                }
            }

            await _ctx.SaveChangesAsync();
        }
    }
}
