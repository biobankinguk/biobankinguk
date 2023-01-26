using Biobanks.Data;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class ServiceOfferingService : ReferenceDataCrudService<ServiceOffering>
    {
        public ServiceOfferingService(ApplicationDbContext db) : base(db) { }

        protected override IQueryable<ServiceOffering> Query()
            => base.Query().Include(x => x.OrganisationServiceOfferings);

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationServiceOfferings.CountAsync(x => x.ServiceOfferingId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationServiceOfferings.AnyAsync(x => x.ServiceOfferingId == id);
    }
}

