using Biobanks.Directory.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class AgeRangeService : ReferenceDataService<AgeRange>
    {
        public AgeRangeService(BiobanksDbContext db) : base(db) { }

        public override Task<int> GetUsageCount(int id)
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> IsInUse(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
