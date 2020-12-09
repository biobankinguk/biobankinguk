using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class StorageTemperatureService : IStorageTemperatureService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public StorageTemperatureService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StorageTemperature>> List()
            => await _db.StorageTemperatures.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public async Task<StorageTemperature> Get(int storageTemperatureId)
            => await _db.StorageTemperatures.FirstOrDefaultAsync(st => st.Id == storageTemperatureId);

        /// <inheritdoc />
        public async Task<StorageTemperature> GetByValue(string value)
            => await _db.StorageTemperatures.FirstOrDefaultAsync(st => st.Value == value);

        /// <inheritdoc />
        public async Task<StorageTemperature> Create(StorageTemperature storageTemperature)
        {
            await _db.StorageTemperatures.AddAsync(storageTemperature);
            await _db.SaveChangesAsync();
            return storageTemperature;
        }

        /// <inheritdoc />
        public async Task Update(StorageTemperature storageTemperature)
        {
            _db.StorageTemperatures.Update(storageTemperature);
            await _db.SaveChangesAsync();
        }
    }
}