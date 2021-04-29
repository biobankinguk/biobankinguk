using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly BiobanksDbContext _db;

        public OrganisationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<AccessCondition> GetAccessCondition(Organisation organisation)
            => await _db.AccessConditions.FirstOrDefaultAsync(x => x.Id == organisation.AccessConditionId);

        public async Task<CollectionType> GetCollectionType(Organisation organisation)
            => await _db.CollectionTypes.FirstOrDefaultAsync(x => x.Id == organisation.CollectionTypeId);
    }
}
