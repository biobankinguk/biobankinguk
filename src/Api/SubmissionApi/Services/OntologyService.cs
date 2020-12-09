using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class OntologyService : IOntologyService
    {
        private readonly Data.SubmissionsDbContext _db;

        /// <inheritdoc />
        public OntologyService(Data.SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Ontology>> List()
            => await _db.Ontologies
                .Include(o => o.OntologyVersions)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<Ontology> Get(int ontologyId)
            => await _db.Ontologies
                .Include(o => o.OntologyVersions)
                .FirstOrDefaultAsync(o => o.Id == ontologyId);

        /// <inheritdoc />
        public async Task<Ontology> GetByValue(string value)
            => await _db.Ontologies.FirstOrDefaultAsync(o => o.Value == value);

        /// <inheritdoc />
        public async Task<Ontology> Create(Ontology ontology)
        {
            await _db.Ontologies.AddAsync(ontology);
            await _db.SaveChangesAsync();
            return ontology;
        }

        /// <inheritdoc />
        public async Task Update(Ontology ontology)
        {
            _db.Ontologies.Update(ontology);
            await _db.SaveChangesAsync();
        }
    }
}