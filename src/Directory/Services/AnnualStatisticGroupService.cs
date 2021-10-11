using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AnnualStatisticGroupService : ReferenceDataService<AnnualStatisticGroup>
    {
        public AnnualStatisticGroupService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AnnualStatisticGroup> Query()
            => base.Query()
                .Include(x => x.AnnualStatistics);

        public override async Task<int> GetUsageCount(int id)
            => await Query()
                .Where(x => x.Id == id)
                .Select(x => x.AnnualStatistics.Count)
                .FirstAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query()
                .Where(x => x.Id == id)
                .SelectMany(x => x.AnnualStatistics)
                .AnyAsync();
    }
}
