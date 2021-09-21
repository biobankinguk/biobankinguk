using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class RegistrationReasonService : ReferenceDataService<RegistrationReason>
    {
        public RegistrationReasonService(BiobanksDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.OrganisationRegistrationReasons.CountAsync(x => x.RegistrationReasonId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.OrganisationRegistrationReasons.AnyAsync(x => x.RegistrationReasonId == id);
    }
}