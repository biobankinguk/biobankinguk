using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class OntologyService : IOntologyService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public OntologyService(BiobanksDbContext db)
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