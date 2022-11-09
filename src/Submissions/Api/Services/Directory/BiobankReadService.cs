using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Identity.Contracts;
using Biobanks.Submissions.Api.Auth.Entities;
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

        private readonly IGenericEFRepository<Collection> _collectionRepository;
        private readonly IGenericEFRepository<CollectionAssociatedData> _collectionAssociatedDataRepository;
        private readonly IGenericEFRepository<CapabilityAssociatedData> _capabilityAssociatedDataRepository;

        private readonly IGenericEFRepository<DiagnosisCapability> _capabilityRepository;
        private readonly IGenericEFRepository<SnomedTag> _snomedTagRepository;
        private readonly IGenericEFRepository<SampleSet> _sampleSetRepository;

        private readonly IGenericEFRepository<Network> _networkRepository;
        private readonly IGenericEFRepository<NetworkUser> _networkUserRepository;
        private readonly IGenericEFRepository<NetworkRegisterRequest> _networkRegisterRequestRepository;
        private readonly IGenericEFRepository<OrganisationNetwork> _networkOrganisationRepository;

        private readonly IGenericEFRepository<Organisation> _organisationRepository;
        private readonly IGenericEFRepository<OrganisationType> _organisationTypeRepository;

        private readonly IGenericEFRepository<MaterialDetail> _materialDetailRepository;
        private readonly IGenericEFRepository<MaterialType> _materialTypeRepository;
        private readonly IGenericEFRepository<OrganisationAnnualStatistic> _organisationAnnualStatisticRepository;
        private readonly IGenericEFRepository<OrganisationRegistrationReason> _organisationRegistrationReasonRepository;
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;
        private readonly IGenericEFRepository<OrganisationUser> _organisationUserRepository;
        private readonly IGenericEFRepository<OrganisationNetwork> _organisationNetworkRepository;
        private readonly IGenericEFRepository<OrganisationRegisterRequest> _organisationRegisterRequestRepository;
        private readonly IGenericEFRepository<PreservationType> _preservationTypeRepository;

        private readonly IGenericEFRepository<TokenValidationRecord> _tokenValidationRecordRepository;
        private readonly IGenericEFRepository<TokenIssueRecord> _tokenIssueRecordRepository;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        private readonly BiobanksDbContext _context;

        public BiobankReadService(
            ILogoStorageProvider logoStorageProvider,

            IGenericEFRepository<Collection> collectionRepository,
            IGenericEFRepository<DiagnosisCapability> capabilityRepository,
            IGenericEFRepository<CapabilityAssociatedData> capabilityAssociatedDataRepository,
            IGenericEFRepository<CollectionAssociatedData> collectionAssociatedDataRepository,
            IGenericEFRepository<SnomedTag> snomedTagRepository,
            IGenericEFRepository<SampleSet> sampleSetRepository,

            IGenericEFRepository<Network> networkRepository,
            IGenericEFRepository<NetworkUser> networkUserRepository,
            IGenericEFRepository<NetworkRegisterRequest> networkRegisterRequestRepository,
            IGenericEFRepository<OrganisationNetwork> networkOrganisationRepository,

            IGenericEFRepository<Organisation> organisationRepository,
            IGenericEFRepository<OrganisationType> organisationTypeRepository,
            IGenericEFRepository<MaterialType> materialTypeRepository,
            IGenericEFRepository<MaterialDetail> materialDetailRepository,
            IGenericEFRepository<OrganisationAnnualStatistic> organisationAnnualStatisticRepository,
            IGenericEFRepository<OrganisationRegistrationReason> organisationRegistrationReasonRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<OrganisationRegisterRequest> organisationRegisterRequestRepository,
            IGenericEFRepository<OrganisationNetwork> organisationNetworkRepository,
            IGenericEFRepository<OrganisationUser> organisationUserRepository,

            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IGenericEFRepository<PreservationType> preservationTypeRepository,

            IGenericEFRepository<TokenValidationRecord> tokenValidationRecordRepository,
            IGenericEFRepository<TokenIssueRecord> tokenIssueRecordRepository,

            BiobanksDbContext context)
        {
            _logoStorageProvider = logoStorageProvider;

            _collectionRepository = collectionRepository;
            _capabilityRepository = capabilityRepository;
            _collectionAssociatedDataRepository = collectionAssociatedDataRepository;
            _capabilityAssociatedDataRepository = capabilityAssociatedDataRepository;
            _snomedTagRepository = snomedTagRepository;
            _sampleSetRepository = sampleSetRepository;

            _networkRepository = networkRepository;
            _networkUserRepository = networkUserRepository;
            _networkRegisterRequestRepository = networkRegisterRequestRepository;
            _networkOrganisationRepository = networkOrganisationRepository;

            _organisationRepository = organisationRepository;
            _organisationTypeRepository = organisationTypeRepository;
            _materialTypeRepository = materialTypeRepository;
            _materialDetailRepository = materialDetailRepository;
            _organisationAnnualStatisticRepository = organisationAnnualStatisticRepository;
            _organisationRegisterRequestRepository = organisationRegisterRequestRepository;

            _organisationRegistrationReasonRepository = organisationRegistrationReasonRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _organisationNetworkRepository = organisationNetworkRepository;
            _organisationUserRepository = organisationUserRepository;
            _preservationTypeRepository = preservationTypeRepository;

            _userManager = userManager;

            _tokenValidationRecordRepository = tokenValidationRecordRepository;
            _tokenIssueRecordRepository = tokenIssueRecordRepository;

            _context = context;
        }

        #endregion

        public async Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId)
            => (await _organisationRepository.ListAsync(
                    false,
                    x => x.OrganisationId == biobankId,
                    null,
                    x => x.Funders))
                .Select(x => x.Funders)
                .FirstOrDefault();

        public async Task<IEnumerable<int>> GetAllSampleSetIdsAsync()
            => (await _sampleSetRepository.ListAsync()).Select(x => x.Id);

        public async Task<IEnumerable<int>> GetAllCapabilityIdsAsync()
            => (await _capabilityRepository.ListAsync()).Select(x => x.DiagnosisCapabilityId);

        public async Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(
            IEnumerable<int> sampleSetIds)
        {
            var sampleSets = await _sampleSetRepository.ListAsync(false,
                x => sampleSetIds.Contains(x.Id) && !x.Collection.Organisation.IsSuspended,
                null,
                x => x.Collection,
                x => x.Collection.OntologyTerm,
                x => x.Collection.Organisation,
                x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Collection.CollectionStatus,
                x => x.Collection.ConsentRestrictions,
                x => x.Collection.AccessCondition,
                x => x.Collection.CollectionType,
                x => x.Collection.AssociatedData.Select(ad => ad.AssociatedDataType),
                x => x.AgeRange,
                x => x.DonorCount,
                x => x.Sex,
                x => x.MaterialDetails,
                x => x.Collection.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.MaterialDetails.Select(y => y.CollectionPercentage),
                x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
                x => x.MaterialDetails.Select(y => y.MaterialType),
                x => x.MaterialDetails.Select(y => y.StorageTemperature),
                x => x.Collection.Organisation.Country,
                x => x.Collection.Organisation.County
            );

            return sampleSets;
        }

        public async Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(
                IEnumerable<int> capabilityIds)
            => await _capabilityRepository.ListAsync(false,
                x => capabilityIds.Contains(x.DiagnosisCapabilityId) && !x.Organisation.IsSuspended,
                null,
                x => x.Organisation,
                x => x.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.OntologyTerm,
                x => x.AssociatedData,
                x => x.SampleCollectionMode
            );

        public async Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexDeletionAsync(
                IEnumerable<int> sampleSetIds)
            => await _sampleSetRepository.ListAsync(false, x => sampleSetIds.Contains(x.Id), null,
                x => x.Collection,
                x => x.Collection.OntologyTerm,
                x => x.Collection.Organisation,
                x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Collection.CollectionStatus,
                x => x.Collection.ConsentRestrictions,
                x => x.Collection.AccessCondition,
                x => x.Collection.CollectionType,
                x => x.Collection.AssociatedData.Select(ad => ad.AssociatedDataType),
                x => x.AgeRange,
                x => x.DonorCount,
                x => x.Sex,
                x => x.MaterialDetails,
                x => x.Collection.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.MaterialDetails.Select(y => y.CollectionPercentage),
                x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
                x => x.MaterialDetails.Select(y => y.MaterialType),
                x => x.MaterialDetails.Select(y => y.StorageTemperature),
                x => x.Collection.Organisation.Country,
                x => x.Collection.Organisation.County
            );

        public async Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(
                IEnumerable<int> capabilityIds)
            => await _capabilityRepository.ListAsync(false,
                x => capabilityIds.Contains(x.DiagnosisCapabilityId),
                null,
                x => x.Organisation,
                x => x.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.OntologyTerm,
                x => x.AssociatedData,
                x => x.SampleCollectionMode
            );

        public async Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm)
            => (await _capabilityRepository.ListAsync(false,
                x => x.OntologyTerm.Value == ontologyTerm)).Select(x => x.DiagnosisCapabilityId);

        public async Task<int> GetIndexableSampleSetCountAsync()
            => (await GetSampleSetsByIdsForIndexingAsync(await GetAllSampleSetIdsAsync())).Count();

        public async Task<int> GetIndexableCapabilityCountAsync()
            => (await GetCapabilitiesByIdsForIndexingAsync(await GetAllCapabilityIdsAsync())).Count();

        public async Task<int> GetSuspendedSampleSetCountAsync()
            => await _sampleSetRepository.CountAsync(
                x => x.Collection.Organisation.IsSuspended);

        public async Task<int> GetSuspendedCapabilityCountAsync()
            => await _capabilityRepository.CountAsync(
                x => x.Organisation.IsSuspended);
        
        public async Task<int> GetSampleSetCountAsync()
            => await _sampleSetRepository.CountAsync();

        public async Task<int> GetCapabilityCountAsync()
            => await _capabilityRepository.CountAsync();

        public async Task<Collection> GetCollectionByIdAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.OntologyTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.ConsentRestrictions,
                x => x.AssociatedData
            )).FirstOrDefault();

        public async Task<bool> IsCollectionFromApi(int id)
            => (await _collectionRepository.CountAsync(x => x.CollectionId == id && x.FromApi)) > 0;

        public async Task<Collection> GetCollectionByIdForIndexingAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.OntologyTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.ConsentRestrictions,
                x => x.AssociatedData,
                x => x.AssociatedData.Select(y => y.AssociatedDataType),
                x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe),
                x => x.SampleSets
            )).FirstOrDefault();

        public async Task<Collection> GetCollectionWithSampleSetsByIdAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.OntologyTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.ConsentRestrictions,
                x => x.AssociatedData.Select(y => y.AssociatedDataType),
                x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe),
                x => x.SampleSets.Select(y => y.Sex),
                x => x.SampleSets.Select(y => y.AgeRange),
                x => x.SampleSets.Select(y => y.MaterialDetails.Select(z => z.MaterialType)),
                x => x.SampleSets.Select(y => y.MaterialDetails.Select(z => z.StorageTemperature))
            )).FirstOrDefault();

        public async Task<IEnumerable<Collection>> ListCollectionsAsync()
        {
            var collections = await _collectionRepository.ListAsync(
                false,
                null,
                null,
                x => x.OntologyTerm,
                x => x.SampleSets.Select(y => y.MaterialDetails));

            return collections;
        }

        public async Task<IEnumerable<Collection>> ListCollectionsAsync(int organisationId)
        {
            var collections = await _collectionRepository.ListAsync(
                false,
                x => x.OrganisationId == organisationId,
                null,
                x => x.OntologyTerm,
                x => x.SampleSets.Select(y => y.MaterialDetails));

            return collections;
        }

        public async Task<SampleSet> GetSampleSetByIdAsync(int id)
            => (await _sampleSetRepository.ListAsync(false, x => x.Id == id, null,
                x => x.Sex,
                x => x.AgeRange,
                x => x.DonorCount,
                x => x.MaterialDetails,
                x => x.MaterialDetails.Select(y => y.CollectionPercentage),
                x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
                x => x.MaterialDetails.Select(y => y.MaterialType),
                x => x.MaterialDetails.Select(y => y.StorageTemperature),
                x => x.MaterialDetails.Select(y => y.ExtractionProcedure)
            )).FirstOrDefault();

        public async Task<SampleSet> GetSampleSetByIdForIndexingAsync(int id)
        {
            try
            {
                var sets = (await _sampleSetRepository.ListAsync(false, x => x.Id == id, null,
                    x => x.Collection,
                    x => x.Collection.OntologyTerm,
                    x => x.Collection.Organisation,
                    x => x.Collection.Organisation.OrganisationNetworks.Select(on => @on.Network),
                    x => x.Collection.CollectionStatus,
                    x => x.Collection.ConsentRestrictions,
                    x => x.Collection.AccessCondition,
                    x => x.Collection.CollectionType,
                    x => x.Collection.AssociatedData.Select(ad => ad.AssociatedDataType),
                    x => x.AgeRange,
                    x => x.DonorCount,
                    x => x.Sex,
                    x => x.MaterialDetails,
                    x => x.Collection.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                    x => x.MaterialDetails.Select(y => y.CollectionPercentage),
                    x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
                    x => x.MaterialDetails.Select(y => y.MaterialType),
                    x => x.MaterialDetails.Select(y => y.StorageTemperature),
                    x => x.Collection.Organisation.Country,
                    x => x.Collection.Organisation.County
                )).FirstOrDefault();

                return sets;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId)
            => _collectionRepository.List(
                false,
                x => x.OrganisationId == biobankId &&
                     x.CollectionId == collectionId).Any();

        public bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId)
            => _sampleSetRepository.List(
                false,
                x => x.Collection.OrganisationId == biobankId &&
                     x.Id == sampleSetId).Any();

        public async Task<DiagnosisCapability> GetCapabilityByIdAsync(int id)
            => (await _capabilityRepository.ListAsync(false,
                x => x.DiagnosisCapabilityId == id,
                null,
                x => x.OntologyTerm,
                x => x.AssociatedData,
                x => x.SampleCollectionMode
            )).FirstOrDefault();

        public async Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id)
            => (await _capabilityRepository.ListAsync(false,
                x => x.DiagnosisCapabilityId == id,
                null,
                x => x.Organisation,
                x => x.Organisation.OrganisationNetworks.Select(on => @on.Network),
                x => x.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.OntologyTerm,
                x => x.AssociatedData,
                x => x.AssociatedData.Select(y => y.AssociatedDataType),
                x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe),
                x => x.SampleCollectionMode
            )).FirstOrDefault();

        public async Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId)
        {
            var capabilities = await _capabilityRepository.ListAsync(
                false,
                x => x.OrganisationId == organisationId,
                null,
                x => x.OntologyTerm,
                x => x.SampleCollectionMode);

            return capabilities;
        }

        public bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId)
            => _capabilityRepository.List(
                false,
                x => x.OrganisationId == biobankId &&
                     x.DiagnosisCapabilityId == capabilityId).Any();

        public IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            return collection.SampleSets
                .SelectMany(x => x.MaterialDetails)
                .Select(x => x.MaterialType.Value)
                .Distinct();
        }

        public async Task<Blob> GetLogoBlobAsync(string logoName)
            => await _logoStorageProvider.GetLogoBlobAsync(logoName);

        #region RefData: Extraction Procedure

        public async Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false)
        => (await _materialTypeRepository.ListAsync(false, x => x.Id == id, null, x => x.ExtractionProcedures))
        .FirstOrDefault()?.ExtractionProcedures
        .Where(x => x.DisplayOnDirectory || !onlyDisplayable)
        .ToList();

        public async Task<int> GetExtractionProcedureMaterialDetailsCount(string id)
            => await _materialDetailRepository.CountAsync(x => x.ExtractionProcedureId == id);

        public async Task<bool> IsExtractionProcedureInUse(string id)
            => (await GetExtractionProcedureMaterialDetailsCount(id) > 0);

        #endregion

        #region RefData: Snomed Tags
        public async Task<IEnumerable<SnomedTag>> ListSnomedTags()
        => await _snomedTagRepository.ListAsync();

        public async Task<SnomedTag> GetSnomedTagByDescription(string description)
            => (await _snomedTagRepository.ListAsync(filter: x => x.Value == description)).SingleOrDefault();

        #endregion

        public async Task<IEnumerable<int>> GetCollectionIdsByOntologyTermAsync(string ontologyTerm)
            => (await _collectionRepository.ListAsync(false,
                x => x.OntologyTerm.Value == ontologyTerm)).Select(x => x.CollectionId);

        public async Task<int> GetMaterialTypeMaterialDetailCount(int id)
            => await _materialDetailRepository.CountAsync(x => x.MaterialTypeId == id);

        public async Task<bool> IsMaterialTypeAssigned(int id)
            => await _context.OntologyTerms
                .Include(x => x.MaterialTypes)
                .Where(x => x.SnomedTag != null && x.SnomedTag.Value == SnomedTags.ExtractionProcedure)
                .AnyAsync(x => x.MaterialTypes.Any(y => y.Id == id));

        public async Task<int> GetServiceOfferingOrganisationCount(int id)
            => (await _organisationServiceOfferingRepository.ListAsync(
            false,
             x => x.ServiceOfferingId == id)).Count();

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
            => await _organisationServiceOfferingRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId,
                null,
                x => x.ServiceOffering);

        public async Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId)
        {
            var adminIds = (await _organisationUserRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId))
                .Select(x => x.OrganisationUserId);

            return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
        }

        public async Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int biobankId)
        {
            // Returns users who have admin role only for this biobank
            // TODO remove the generic repo when upgrading to netcore, as it doesn't support groupby fully
            var admins = await _organisationUserRepository.ListAsync(false);
            var adminIds = admins.GroupBy(a => a.OrganisationUserId)
                .Where(g => g.Count() == 1)
                .Select(a => a.FirstOrDefault(ai => ai.OrganisationId == biobankId))
                .Select(ou => ou?.OrganisationUserId);

            return await _userManager.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        }

        public List<Organisation> GetOrganisations() => _organisationRepository.List(false, x => x.IsSuspended == false, x => x.OrderBy(c => c.Name)).ToList();

        public async Task<string> GetUnusedTokenByUser(string biobankUserId)
        {
            // Check most recent token record
            var tokenIssue = (await _tokenIssueRecordRepository.ListAsync(
                                        false,
                                        x => x.UserId.Contains(biobankUserId),
                                        x => x.OrderBy(c => c.IssueDate))).FirstOrDefault();

            // Check validation records
            var tokenValidation = await _tokenValidationRecordRepository.ListAsync(
                                            false,
                                            x => x.UserId.Contains(biobankUserId));

            List<string> token = tokenValidation.Select(t => t.Token).ToList();
            DateTime now = DateTime.Now;

            if (tokenIssue.Equals(null) || token.Contains(tokenIssue.Token) || tokenIssue.IssueDate < now.AddHours(-20))
            {
                return await _userManager.GeneratePasswordResetTokenAsync(biobankUserId);
            }
            else
            {
                return tokenIssue.Token;
            }
        }
    }
}