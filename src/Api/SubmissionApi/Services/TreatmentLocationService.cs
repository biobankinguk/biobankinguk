using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class TreatmentLocationService : ITreatmentLocationService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public TreatmentLocationService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TreatmentLocation>> List()
            => await _db.TreatmentLocations.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public async Task<TreatmentLocation> Get(int treatmentId)
            => await _db.TreatmentLocations.FirstOrDefaultAsync(tl => tl.Id == treatmentId);

        /// <inheritdoc />
        public async Task<TreatmentLocation> GetByValue(string value)
            => await _db.TreatmentLocations.FirstOrDefaultAsync(tl => tl.Value == value);

        /// <inheritdoc />
        public async Task<TreatmentLocation> Create(TreatmentLocation treatmentLocation)
        {
            await _db.TreatmentLocations.AddAsync(treatmentLocation);
            await _db.SaveChangesAsync();
            return treatmentLocation;
        }

        /// <inheritdoc />
        public async Task Update(TreatmentLocation treatmentLocation)
        {
            _db.TreatmentLocations.Update(treatmentLocation);
            await _db.SaveChangesAsync();
        }
    }
}