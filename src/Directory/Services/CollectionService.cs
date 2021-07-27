using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Directory.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly BiobanksDbContext _db;

        public CollectionService(BiobanksDbContext db)
        {
            _db = db;
        }
    }
}
