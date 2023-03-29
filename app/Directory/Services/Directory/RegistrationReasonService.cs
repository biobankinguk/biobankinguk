using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class RegistrationReasonService : ReferenceDataCrudService<RegistrationReason>
    {
        public RegistrationReasonService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationRegistrationReasons.CountAsync(x => x.RegistrationReasonId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationRegistrationReasons.AnyAsync(x => x.RegistrationReasonId == id);
    }
}
