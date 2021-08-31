﻿using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using System;
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
        public async Task<Publication> Update(string publicationId, int organisationId, Action<Publication> updates)
        {
            var publication = await _db.Publications
                .FirstOrDefaultAsync(x => x.PublicationId == publicationId && x.OrganisationId == organisationId);

            if (publication is null)
                return null;

            // Apply Updates To Tracked Organisation
            updates(publication);

            // Push Changes
            await _db.SaveChangesAsync();

            // Remove Tracking Of Object, Such That Updates Only Occur Within This Scope
            _db.Entry(publication).State = EntityState.Detached;

            return publication;
        }
    }
}
