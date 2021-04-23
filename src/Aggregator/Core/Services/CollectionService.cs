using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly BiobanksDbContext _db;

        public CollectionService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task AddCollectionAsync(Collection collection)
        {
            try
            {
                await _db.Collections.AddAsync(collection);
                await _db.SaveChangesAsync();
            }
            catch
            {
            }
        }

        public async Task UpdateCollectionAsync(Collection collection)
        {
            _db.Update(collection);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCollectionAsync(Collection collection)
            => await _db.Collections.Where(x => x.CollectionId == collection.CollectionId).DeleteAsync();

    }
}
