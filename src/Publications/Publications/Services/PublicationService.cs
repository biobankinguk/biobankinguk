﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Publications.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
