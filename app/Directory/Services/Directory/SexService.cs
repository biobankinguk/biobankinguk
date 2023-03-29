using System;
using Biobanks.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class SexService : ReferenceDataCrudService<Sex>
    {
        public SexService(ApplicationDbContext db) : base(db) { }

        public override async Task<int> GetUsageCount(int id)
            => await _db.SampleSets.CountAsync(x => x.SexId == id);

        public override async Task<bool> IsInUse(int id)
            => await _db.SampleSets.AnyAsync(x => x.SexId == id);
    }
}

