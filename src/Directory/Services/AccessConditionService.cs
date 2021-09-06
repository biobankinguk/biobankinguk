using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Linq;

namespace Biobanks.Directory.Services
{
    public class AccessConditionService : ReferenceDataService<AccessCondition>
    {
        public AccessConditionService(BiobanksDbContext db) : base(db) { }

        protected override IQueryable<AccessCondition> Query()
            => base.Query().Include(x => x.Organisations);
    }
}
