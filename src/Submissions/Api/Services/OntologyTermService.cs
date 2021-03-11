using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class OntologyTermService : IOntologyTermService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public OntologyTermService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OntologyTerm>> List()
            => await _db.OntologyTerms
                .Include(st => st.SnomedTag)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<OntologyTerm> Get(string ontologyTermId)
            => await _db.OntologyTerms
                .Include(st => st.SnomedTag)
                .FirstOrDefaultAsync(st => st.Id == ontologyTermId);

        /// <inheritdoc />
        public async Task<OntologyTerm> GetByValue(string value)
            => await _db.OntologyTerms.FirstOrDefaultAsync(st => st.Value == value);

        /// <inheritdoc />
        public async Task<OntologyTerm> Create(OntologyTerm ontologyTerm)
        {
            await _db.OntologyTerms.AddAsync(ontologyTerm);
            await _db.SaveChangesAsync();
            return ontologyTerm;
        }

        /// <inheritdoc />
        public async Task Create(IList<OntologyTerm> ontologyTerms)
        {
            await _db.OntologyTerms.AddRangeAsync(ontologyTerms);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Update(OntologyTerm ontologyTerm)
        {
            _db.OntologyTerms.Update(ontologyTerm);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Update(List<OntologyTerm> ontologyTerms)
        {
            _db.OntologyTerms.UpdateRange(ontologyTerms);
            await _db.SaveChangesAsync();
        }
    }
}