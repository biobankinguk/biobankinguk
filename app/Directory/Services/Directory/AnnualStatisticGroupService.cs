using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
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

