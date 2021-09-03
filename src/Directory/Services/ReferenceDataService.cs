using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly BiobanksDbContext _db;

        public ReferenceDataService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<int> Count<T>() where T : ReferenceDataBase
            => await _db.Set<T>().CountAsync();

        /// <inheritdoc/>
        public async Task<bool> Exists<T>(string value) where T : ReferenceDataBase
            => await _db.Set<T>().AnyAsync(x => x.Value == value);

        /// <inheritdoc/>
        public async Task<T> Get<T>(int id) where T : ReferenceDataBase
            => await _db.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        /// <inheritdoc/>
        public async Task<ICollection<T>> List<T>() where T : ReferenceDataBase
            => await _db.Set<T>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
    }
}
