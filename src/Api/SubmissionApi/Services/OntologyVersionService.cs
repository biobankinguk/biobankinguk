using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class OntologyVersionService : IOntologyVersionService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public OntologyVersionService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OntologyVersion>> List(int? ontologyId)
            => await _db.OntologyVersions
                .Include(o => o.Ontology)
                .Where(ov => ontologyId == null || ov.OntologyId == ontologyId)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<OntologyVersion> Get(int ontologyVersionId)
            => await _db.OntologyVersions
                .Include(o => o.Ontology)
                .FirstOrDefaultAsync(o => o.Id == ontologyVersionId);

        /// <inheritdoc />
        public async Task<OntologyVersion> GetByValue(string value)
            => await _db.OntologyVersions.FirstOrDefaultAsync(o => o.Value == value);

        /// <inheritdoc />
        public async Task<OntologyVersion> Create(OntologyVersion ontologyVersion)
        {
            var ontology = _db.Ontologies.FirstOrDefault(o => o.Value == ontologyVersion.Ontology.Value);

            ontologyVersion.Ontology = ontology ?? throw new KeyNotFoundException();

            await _db.OntologyVersions.AddAsync(ontologyVersion);
            await _db.SaveChangesAsync();
            return ontologyVersion;
        }

        /// <inheritdoc />
        public async Task Update(OntologyVersion ontologyVersion)
        {
            _db.OntologyVersions.Update(ontologyVersion);
            await _db.SaveChangesAsync();
        }
    }
}