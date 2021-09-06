using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data.ReferenceData;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class ReferenceDataService<T> : IReferenceDataService<T> where T : ReferenceDataBase
    {
        private readonly BiobanksDbContext _db;

        public ReferenceDataService(BiobanksDbContext db)
        {
            _db = db;
        }

        protected virtual IQueryable<T> Query()
            => _db.Set<T>().AsQueryable();

        public async Task Add(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<int> Count()
            => await Query().CountAsync();

        public async Task<bool> Exists(string value)
            => await Query().AnyAsync(x => x.Value == value);

        public async Task<T> Get(int id)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<ICollection<T>> List()
            => await Query()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
    }

    public class AccessConditionService : ReferenceDataService<AccessCondition>
    {
        public AccessConditionService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => base.Query().Include(x => x.Organisations);
    }
}
