using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;

namespace Biobanks.Aggregator.Core.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly BiobanksDbContext _db;

        public AggregationService(BiobanksDbContext db)
        {
            _db = db;
        }
    }
}
