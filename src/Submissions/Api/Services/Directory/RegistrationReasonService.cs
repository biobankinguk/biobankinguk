using Biobanks.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory
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