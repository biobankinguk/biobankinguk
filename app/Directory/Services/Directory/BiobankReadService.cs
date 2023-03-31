using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Directory.Services.Directory.Constants;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class BiobankReadService : IBiobankReadService
    {
        #region Properties and ctor
        private readonly ILogoStorageProvider _logoStorageProvider;

        private readonly ApplicationDbContext _db;

        public BiobankReadService(
            ILogoStorageProvider logoStorageProvider,
            ApplicationDbContext db)
        {
            _logoStorageProvider = logoStorageProvider;


            _db = db;
        }

        #endregion
    


        public bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId)
            => _db.Collections
            .AsNoTracking()
            .Where(x => x.OrganisationId == biobankId && x.CollectionId == collectionId)
            .Any();

        public bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId)
            => _db.SampleSets
            .AsNoTracking()
            .Where(x => x.Collection.OrganisationId == biobankId &&
                     x.Id == sampleSetId)
            .Any();

    public bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId)
            => _db.DiagnosisCapabilities.AsNoTracking()
            .Where(x => x.OrganisationId == biobankId &&  x.DiagnosisCapabilityId == capabilityId).Any();


        #region RefData: Extraction Procedure
        
        public async Task<int> GetExtractionProcedureMaterialDetailsCount(string id)
            => await _db.MaterialDetails.CountAsync(x => x.ExtractionProcedureId == id);

        public async Task<bool> IsExtractionProcedureInUse(string id)
            => (await GetExtractionProcedureMaterialDetailsCount(id) > 0);

        #endregion
        

        public async Task<int> GetMaterialTypeMaterialDetailCount(int id)
            => await _db.MaterialDetails.CountAsync(x => x.MaterialTypeId == id);

        public async Task<bool> IsMaterialTypeAssigned(int id)
            => await _db.OntologyTerms
                .Include(x => x.MaterialTypes)
                .Where(x => x.SnomedTag != null && x.SnomedTag.Value == SnomedTags.ExtractionProcedure)
                .AnyAsync(x => x.MaterialTypes.Any(y => y.Id == id));

        public async Task<int> GetServiceOfferingOrganisationCount(int id)
            => await _db.OrganisationServiceOfferings
                .AsNoTracking()
                .Where( x => x.ServiceOfferingId == id)
                .CountAsync();
        
    }
}
