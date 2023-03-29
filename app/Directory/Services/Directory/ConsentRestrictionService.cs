using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Biobanks.Submissions.Api.Services.Directory
{
    public class ConsentRestrictionService : ReferenceDataCrudService<ConsentRestriction>
    {
        public ConsentRestrictionService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<ConsentRestriction> Query()
            => base.Query().Include(x => x.Collections);

        public override async Task<int> GetUsageCount(int id)
            => await Query().Where(x => x.Id == id).Where(x => x.Collections.Count() > 0).CountAsync();

        public override async Task<bool> IsInUse(int id)
            => await Query().Where(x => x.Id == id).Where(x => x.Collections.Count() > 0).AnyAsync();
    }
}
