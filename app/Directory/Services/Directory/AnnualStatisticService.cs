using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class AnnualStatisticService : ReferenceDataCrudService<AnnualStatistic>
    {
        public AnnualStatisticService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<AnnualStatistic> Query()
            => base.Query()
                .Include(x => x.AnnualStatisticGroup)
                .Include(x => x.OrganisationAnnualStatistics);

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationAnnualStatistics.CountAsync(x => x.AnnualStatisticId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationAnnualStatistics.AnyAsync(x => x.AnnualStatisticId == id);
        
        public override async Task<AnnualStatistic> Update(AnnualStatistic entity)
        {
            var existing = await base.Update(entity);
            existing.AnnualStatisticGroupId = entity.AnnualStatisticGroupId;
            await _db.SaveChangesAsync();
        
            return existing;
        }
    }
}

