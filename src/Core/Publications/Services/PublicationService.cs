﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Publications.Core.Services.Dto;
using Biobanks.Entities.Data;
using Biobanks.Data;
using Biobanks.Publications.Core.Services.Contracts;
using System;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Publications.Core.Services
{
    public class PublicationService : IPublicationService
    {
        private BiobanksDbContext _db;

        public PublicationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int organisationId)
            => await _db.Publications
                .Where(x => x.OrganisationId == organisationId && x.Accepted == true)
                .Where(a => a.AnnotationsSynced == null || a.AnnotationsSynced < DateTime.Today.AddMonths(-1))
                .ToListAsync();

        public async Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications)
        {
            var existingPublications = _db.Publications.Where(x => x.OrganisationId == organisationId);

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
                    _db.Add(newer);
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

                    _db.Update(older);
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
