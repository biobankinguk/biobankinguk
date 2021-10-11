using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AnnualStatisticService : ReferenceDataService<AnnualStatistic>
    {
        public AnnualStatisticService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AnnualStatistic> Query()
            => base.Query()
                .Include(x => x.AnnualStatisticGroup)
                .Include(x => x.OrganisationAnnualStatistics);

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationAnnualStatistics.CountAsync(x => x.AnnualStatisticId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationAnnualStatistics.AnyAsync(x => x.AnnualStatisticId == id);
    }
}
