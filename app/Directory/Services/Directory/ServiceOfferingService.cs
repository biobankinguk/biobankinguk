using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
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

