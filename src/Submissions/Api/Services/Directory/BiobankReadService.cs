using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Constants;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankReadService : IBiobankReadService
    {
        #region Properties and ctor
        private readonly ILogoStorageProvider _logoStorageProvider;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public BiobankReadService(
            ILogoStorageProvider logoStorageProvider,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _logoStorageProvider = logoStorageProvider;


            _context = context;
        }

        #endregion
    


        public bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId)
            => _context.Collections
            .AsNoTracking()
            .Where(x => x.OrganisationId == biobankId && x.CollectionId == collectionId)
            .Any();

        public bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId)
            => _context.SampleSets
            .AsNoTracking()
            .Where(x => x.Collection.OrganisationId == biobankId &&
                     x.Id == sampleSetId)
            .Any();

    public bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId)
            => _context.DiagnosisCapabilities.AsNoTracking()
            .Where(x => x.OrganisationId == biobankId &&  x.DiagnosisCapabilityId == capabilityId).Any();


        #region RefData: Extraction Procedure
        
        public async Task<int> GetExtractionProcedureMaterialDetailsCount(string id)
            => await _materialDetailRepository.CountAsync(x => x.ExtractionProcedureId == id);

        public async Task<bool> IsExtractionProcedureInUse(string id)
            => (await GetExtractionProcedureMaterialDetailsCount(id) > 0);

        #endregion
        

        public async Task<int> GetMaterialTypeMaterialDetailCount(int id)
            => await _context.MaterialDetails.CountAsync(x => x.MaterialTypeId == id);

        public async Task<bool> IsMaterialTypeAssigned(int id)
            => await _context.OntologyTerms
                .Include(x => x.MaterialTypes)
                .Where(x => x.SnomedTag != null && x.SnomedTag.Value == SnomedTags.ExtractionProcedure)
                .AnyAsync(x => x.MaterialTypes.Any(y => y.Id == id));

        public async Task<int> GetServiceOfferingOrganisationCount(int id)
            => await _context.OrganisationServiceOfferings
                .AsNoTracking()
                .Where( x => x.ServiceOfferingId == id)
                .CountAsync();
        
    }
}
