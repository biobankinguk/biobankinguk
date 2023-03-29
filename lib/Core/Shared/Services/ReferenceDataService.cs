using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Shared.Services
{
    public  class ReferenceDataService<T> : IReferenceDataService<T> where T : BaseReferenceData
    {
        protected readonly ApplicationDbContext _db;

        public ReferenceDataService(ApplicationDbContext db)
        {
            _db = db;
        }
        protected virtual IQueryable<T> Query()
            => _db.Set<T>().AsQueryable();

        public async Task<ICollection<T>> List(string wildcard)
            => await Query()
                .AsNoTracking()
                .Where(x => x.Value.Contains(wildcard))
                .OrderBy(x => x.SortOrder)
                .ToListAsync(); 
        public async Task<ICollection<T>> List()
            => await List(string.Empty);

  }
}
