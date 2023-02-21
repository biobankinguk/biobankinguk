using System;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class AnnualStatisticGroupService : ReferenceDataCrudService<AnnualStatisticGroup>
    {
        public AnnualStatisticGroupService(ApplicationDbContext db) : base(db) { }

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

