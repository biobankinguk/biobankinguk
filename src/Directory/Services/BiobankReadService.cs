using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Directory.Data.Caching;
using Directory.Data.Repositories;
using Biobanks.Identity.Contracts;
using Directory.Search.Legacy;
using Directory.Search.Constants;
using Biobanks.Identity.Data.Entities;
using Directory.Services.Dto;
using Directory.Services.Contracts;
using Microsoft.AspNet.Identity;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;

namespace Directory.Services
{
    public class BiobankReadService : IBiobankReadService
    {
        #region Properties and ctor

        private readonly ILogoStorageProvider _logoStorageProvider;

        private readonly IGenericEFRepository<Collection> _collectionRepository;
        private readonly IGenericEFRepository<CollectionAssociatedData> _collectionAssociatedDataRepository;
        private readonly IGenericEFRepository<CapabilityAssociatedData> _capabilityAssociatedDataRepository;
     
        private readonly IGenericEFRepository<DiagnosisCapability> _capabilityRepository;
        private readonly IGenericEFRepository<AccessCondition> _accessConditionRepository;
        private readonly IGenericEFRepository<CollectionType> _collectionTypeRepository;
        private readonly IGenericEFRepository<CollectionStatus> _collectionStatusRepository;
        private readonly IGenericEFRepository<CollectionPoint> _collectionPointRepository;
        private readonly IGenericEFRepository<CollectionPercentage> _collectionPercentageRepository;
        private readonly IGenericEFRepository<CollectionSampleSet> _collectionSampleSetRepository;
        private readonly IGenericEFRepository<ConsentRestriction> _collectionConsentRestrictionRepository;
        private readonly IGenericEFRepository<HtaStatus> _htaStatusRepository;
        private readonly IGenericEFRepository<SnomedTerm> _snomedTermRepository;
        private readonly IGenericEFRepository<CollectionSampleSet> _sampleSetRepository;
        private readonly IGenericEFRepository<Config> _siteConfigRepository;
        private readonly IGenericEFRepository<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameModelRepository;

        private readonly IGenericEFRepository<Network> _networkRepository;
        private readonly IGenericEFRepository<NetworkUser> _networkUserRepository;
        private readonly IGenericEFRepository<NetworkRegisterRequest> _networkRegisterRequestRepository;
        private readonly IGenericEFRepository<SopStatus> _sopStatusRepository;
        private readonly IGenericEFRepository<OrganisationNetwork> _networkOrganisationRepository;

        private readonly IGenericEFRepository<Organisation> _organisationRepository;
        private readonly IGenericEFRepository<OrganisationType> _organisationTypeRepository;
        private readonly IGenericEFRepository<Funder> _funderRepository;

        private readonly IGenericEFRepository<AssociatedDataType> _associatedDataTypeRepository;
        private readonly IGenericEFRepository<AssociatedDataTypeGroup> _associatedDataTypeGroupRepository;
        private readonly IGenericEFRepository<Sex> _sexRepository;
        private readonly IGenericEFRepository<AgeRange> _ageRangeRepository;
        private readonly IGenericEFRepository<DonorCount> _donorCountRepository;
        private readonly IGenericEFRepository<MaterialDetail> _materialDetailsRepository;
        private readonly IGenericEFRepository<MaterialType> _materialTypeRepository;
        private readonly IGenericEFRepository<MaterialDetail> _materialDetailRepository;
        private readonly IGenericEFRepository<OrganisationAnnualStatistic> _organisationAnnualStatisticRepository;
        private readonly IGenericEFRepository<OrganisationRegistrationReason> _organisationRegistrationReasonRepository;
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;
        private readonly IGenericEFRepository<OrganisationUser> _organisationUserRepository;
        private readonly IGenericEFRepository<OrganisationNetwork> _organisationNetworkRepository;
        private readonly IGenericEFRepository<OrganisationRegisterRequest> _organisationRegisterRequestRepository;
        private readonly IGenericEFRepository<RegistrationReason> _registrationReasonRepository;
        private readonly IGenericEFRepository<ServiceOffering> _serviceOfferingRepository;
        private readonly IGenericEFRepository<StorageTemperature> _storageTemperatureRepository;
        private readonly IGenericEFRepository<CollectionPercentage> _collectionPercentage;
        private readonly IGenericEFRepository<MacroscopicAssessment> _macroscopicAssessmentRepository;
        private readonly IGenericEFRepository<SampleCollectionMode> _sampleCollectionModeRepository;


        private readonly IGenericEFRepository<County> _countyRepository;
        private readonly IGenericEFRepository<Country> _countryRepository;

        private readonly IGenericEFRepository<AnnualStatisticGroup> _annualStatisticGroupRepository;
        private readonly IGenericEFRepository<AnnualStatistic> _annualStatisticRepository;

        private readonly IGenericEFRepository<Publication> _publicationRepository;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        private readonly ICacheProvider _cacheProvider;

        private readonly ISearchProvider _searchProvider;

        public BiobankReadService(
            ILogoStorageProvider logoStorageProvider,

            IGenericEFRepository<Collection> collectionRepository,
            IGenericEFRepository<DiagnosisCapability> capabilityRepository,
            IGenericEFRepository<CapabilityAssociatedData> capabilityAssociatedDataRepository,
            IGenericEFRepository<CollectionAssociatedData> collectionAssociatedDataRepository,
            IGenericEFRepository<AccessCondition> accessConditionRepository,
            IGenericEFRepository<CollectionType> collectionTypeRepository,
            IGenericEFRepository<CollectionStatus> collectionStatusRepository,
            IGenericEFRepository<CollectionPoint> collectionPointRepository,
            IGenericEFRepository<CollectionPercentage> collectionPercentageRepository,
            IGenericEFRepository<CollectionSampleSet> collectionSampleSetRepository,
            IGenericEFRepository<ConsentRestriction> collectionConsentRestrictionRepository,
            IGenericEFRepository<HtaStatus> htaStatusRepository,
            IGenericEFRepository<SnomedTerm> snomedTermRepository,
            IGenericEFRepository<CollectionSampleSet> sampleSetRepository,
            IGenericEFRepository<Config> siteConfigRepository,
            IGenericEFRepository<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameModelRepository,
            IGenericEFRepository<AssociatedDataTypeGroup> associatedDataTypeGroupRepository,

            IGenericEFRepository<Network> networkRepository,
            IGenericEFRepository<NetworkUser> networkUserRepository,
            IGenericEFRepository<NetworkRegisterRequest> networkRegisterRequestRepository,
            IGenericEFRepository<SopStatus> sopStatusRepository,
            IGenericEFRepository<OrganisationNetwork> networkOrganisationRepository,

            IGenericEFRepository<Organisation> organisationRepository,
            IGenericEFRepository<OrganisationType> organisationTypeRepository,
            IGenericEFRepository<AssociatedDataType> associatedDataTypeRepository,
            IGenericEFRepository<Sex> sexRepository,
            IGenericEFRepository<AgeRange> ageRangeRepository,
            IGenericEFRepository<DonorCount> donorCountRepository,
            IGenericEFRepository<MaterialDetail> materialDetailsRepository,
            IGenericEFRepository<MaterialType> materialTypeRepository,
            IGenericEFRepository<MaterialDetail> materialDetailRepository,
            IGenericEFRepository<OrganisationAnnualStatistic> organisationAnnualStatisticRepository,
            IGenericEFRepository<OrganisationRegistrationReason> organisationRegistrationReasonRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<OrganisationUser> organisationUserRepository,
            IGenericEFRepository<OrganisationNetwork> organisationNetworkRepository,
            IGenericEFRepository<OrganisationRegisterRequest> organisationRegisterRequestRepository,
            IGenericEFRepository<RegistrationReason> registrationReasonRepository,
            IGenericEFRepository<ServiceOffering> serviceOfferingRepository,
         
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IGenericEFRepository<StorageTemperature> storageTemperatureRepository,
            IGenericEFRepository<CollectionPercentage> collectionPercentage,
            IGenericEFRepository<MacroscopicAssessment> macroscopicAssessmentRepository,
            IGenericEFRepository<SampleCollectionMode> sampleCollectionModeRepository,

            ICacheProvider cacheProvider,

            ISearchProvider searchProvider,

            IGenericEFRepository<Funder> funderRepository, 
            IGenericEFRepository<County> countyRepository,
            IGenericEFRepository<Country> countryRepository, 
            IGenericEFRepository<AnnualStatisticGroup> annualStatisticGroupRepository,
            IGenericEFRepository<AnnualStatistic> annualStatisticRepository,
            IGenericEFRepository<Publication> publicationRespository)
        {
            _logoStorageProvider = logoStorageProvider;

            _collectionRepository = collectionRepository;
            _capabilityRepository = capabilityRepository;
            _collectionAssociatedDataRepository = collectionAssociatedDataRepository;
            _capabilityAssociatedDataRepository = capabilityAssociatedDataRepository;
            _accessConditionRepository = accessConditionRepository;
            _collectionTypeRepository = collectionTypeRepository;
            _collectionStatusRepository = collectionStatusRepository;
            _collectionPointRepository = collectionPointRepository;
            _collectionPercentageRepository = collectionPercentageRepository;
            _collectionSampleSetRepository = collectionSampleSetRepository;
            _collectionConsentRestrictionRepository = collectionConsentRestrictionRepository;
            _htaStatusRepository = htaStatusRepository;
            _snomedTermRepository = snomedTermRepository;
            _sampleSetRepository = sampleSetRepository;
            _siteConfigRepository = siteConfigRepository;
            _associatedDataProcurementTimeFrameModelRepository = associatedDataProcurementTimeFrameModelRepository;
            _associatedDataTypeGroupRepository = associatedDataTypeGroupRepository;

            _networkRepository = networkRepository;
            _networkUserRepository = networkUserRepository;
            _networkRegisterRequestRepository = networkRegisterRequestRepository;
            _sopStatusRepository = sopStatusRepository;
            _networkOrganisationRepository = networkOrganisationRepository;

            _organisationRepository = organisationRepository;
            _organisationTypeRepository = organisationTypeRepository;
            _sexRepository = sexRepository;
            _ageRangeRepository = ageRangeRepository;
            _donorCountRepository = donorCountRepository;
            _materialDetailsRepository = materialDetailsRepository;
            _materialTypeRepository = materialTypeRepository;
            _materialDetailRepository = materialDetailRepository;
            _organisationAnnualStatisticRepository = organisationAnnualStatisticRepository;
            _organisationRegistrationReasonRepository = organisationRegistrationReasonRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _organisationUserRepository = organisationUserRepository;
            _organisationNetworkRepository = organisationNetworkRepository;
            _organisationRegisterRequestRepository = organisationRegisterRequestRepository;
            _registrationReasonRepository = registrationReasonRepository;
            _serviceOfferingRepository = serviceOfferingRepository;

            _storageTemperatureRepository = storageTemperatureRepository;
            _collectionPercentage = collectionPercentage;
            _macroscopicAssessmentRepository = macroscopicAssessmentRepository;
            _sampleCollectionModeRepository = sampleCollectionModeRepository;

            _userManager = userManager;

            _cacheProvider = cacheProvider;

            _searchProvider = searchProvider;
            _funderRepository = funderRepository;
            _countyRepository = countyRepository;
            _countryRepository = countryRepository;
            _annualStatisticGroupRepository = annualStatisticGroupRepository;
            _annualStatisticRepository = annualStatisticRepository;
            _associatedDataTypeRepository = associatedDataTypeRepository;

            _publicationRepository = publicationRespository;
        }

        #endregion

        public async Task<OrganisationType> GetBiobankOrganisationTypeAsync()
            //if we ever have more types, maybe a type service could provide
            //GetOrganisationTypeByDescription(string description)
            => (await _organisationTypeRepository.ListAsync(
                false,
                x => x.Description == "Biobank")).FirstOrDefault();

        public async Task<Network> GetNetworkByIdAsync(int networkId)
            => (await _networkRepository.ListAsync(
                false,
                x => x.NetworkId == networkId,
                null,
                x => x.SopStatus)).FirstOrDefault();

        public async Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId)
        {
            var adminIds = (await _networkUserRepository.ListAsync(
                false,
                x => x.NetworkId == networkId))
                .Select(x => x.NetworkUserId);

            return _userManager.Users.AsNoTracking().Where(x => adminIds.Contains(x.Id));
        }

        public async Task<Network> GetNetworkByNameAsync(string networkName)
            => (await _networkRepository.ListAsync(
                false,
                x => x.Name == networkName)).FirstOrDefault();

        public async Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "",
                bool includeSuspended = true)
            => await _organisationRepository.ListAsync(
                false,
                x => x.Name.Contains(wildcard) &&
                (includeSuspended || x.IsSuspended == false));

        public async Task<bool> IsBiobankSuspendedAsync(int biobankId)
            => await _organisationRepository.CountAsync(
                   x => x.OrganisationId == biobankId &&
                        x.IsSuspended)
               > 0;

        public async Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId)
            => await IsBiobankSuspendedAsync(
                (await _capabilityRepository.GetByIdAsync(capabilityId))
                    .OrganisationId);

        public async Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId)
            => await IsBiobankSuspendedAsync(
                (await _collectionRepository.GetByIdAsync(collectonId))
                    .OrganisationId);

        public async Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId)
            => await IsCollectionBiobankSuspendedAsync(
                (await _sampleSetRepository.GetByIdAsync(sampleSetId))
                    .CollectionId);

        public async Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId)
            => (await _organisationRepository.ListAsync(
                    false,
                    x => x.OrganisationId == biobankId,
                    null,
                    x => x.Funders))
                .Select(x => x.Funders)
                .FirstOrDefault();

        public async Task<Funder> GetFunderbyName(string name)
            => (await _funderRepository.ListAsync(false, x => x.Name == name)).SingleOrDefault();

        public async Task<IEnumerable<Funder>> ListFundersAsync(string wildcard) =>
            (await _funderRepository.ListAsync(
                false,
                x => x.Name.Contains(wildcard)))
            .ToList();

        #region RefData: County
        public async Task<ICollection<County>> ListCountiesAsync() =>
            (await _countyRepository.ListAsync(false, null, x => x.OrderBy(c => c.Name))).ToList();

        public async Task<bool> ValidCountyAsync(string countyName)
            => (await _countyRepository.ListAsync(false, x => x.Name == countyName)).Any();

        public async Task<bool> IsCountyInUse(int id)
            => (await GetCountyUsageCount(id)) > 0;

        public async Task<int> GetCountyUsageCount(int id)
            => (await _organisationRepository.ListAsync(false, x => x.CountyId == id)).Count();
        #endregion

        public async Task<ICollection<Country>> ListCountriesAsync() =>
            (await _countryRepository.ListAsync(false, null, x => x.OrderBy(c => c.Name))).ToList();

        public async Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync()
        {
            var organisations = await _organisationRepository.ListAsync(
                false,
                b => !b.IsSuspended,
                null,
                b => b.Collections,
                b => b.DiagnosisCapabilities,
                b => b.OrganisationUsers
            );

            var organisationUsers = await _organisationUserRepository.ListAsync(false);
            var identityUsers = await _userManager.Users.ToListAsync();

            return (from organisation in organisations
                let organisationUserIds = organisationUsers.Where(ou => ou.OrganisationId == organisation.OrganisationId).Select(ou => ou.OrganisationUserId)
                let organisationIdentityUsers = identityUsers.Where(iu => organisationUserIds.Contains(iu.Id) && iu.LastLogin.HasValue)
                let lastLoginUser = organisationIdentityUsers.OrderByDescending(iu => iu.LastLogin).FirstOrDefault()
                select new BiobankActivityDTO
                {
                    OrganisationId = organisation.OrganisationId,
                    Name = organisation.Name,
                    ContactEmail = organisation.ContactEmail,
                    LastUpdated = organisation.LastUpdated,
                    LastCapabilityUpdated = organisation.DiagnosisCapabilities.OrderByDescending(c => c.LastUpdated).FirstOrDefault()?.LastUpdated,
                    LastCollectionUpdated = organisation.Collections.OrderByDescending(c => c.LastUpdated).FirstOrDefault()?.LastUpdated,
                    LastAdminLoginEmail = lastLoginUser?.Email,
                    LastAdminLoginTime = lastLoginUser?.LastLogin
                }).ToList();
        }

        public async Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdAsync(int networkId)
        {
            var networkBiobankIds = await GetBiobankIdsByNetworkIdAsync(networkId);

            return await _organisationRepository.ListAsync(
                false,
                x => networkBiobankIds.Contains(x.OrganisationId) && !x.IsSuspended);
        }

        public async Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdForIndexingAsync(int networkId)
        {
            var networkBiobankIds = await GetBiobankIdsByNetworkIdAsync(networkId);

            return await _organisationRepository.ListAsync(
                false,
                x => networkBiobankIds.Contains(x.OrganisationId) && !x.IsSuspended,
                null,
                b => b.Collections,
                b => b.Collections.Select(c => c.SampleSets),
                b => b.DiagnosisCapabilities);
        }

        private async Task<IEnumerable<int>> GetBiobankIdsByNetworkIdAsync(int networkId)
            => (await _networkOrganisationRepository.ListAsync(
                false,
                x => x.NetworkId == networkId)).Select(x => x.OrganisationId);

        public async Task<IEnumerable<Network>> ListNetworksAsync()
            => await _networkRepository.ListAsync(false, null, null, n => n.OrganisationNetworks);

        public async Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync()
        {
            var type = await GetBiobankOrganisationTypeAsync();

            //Show all that are "open"
            //(i.e. no action taken (accept, decline, create...))
            return await _organisationRegisterRequestRepository.ListAsync(
                false,
                x =>
                    x.AcceptedDate == null && x.OrganisationCreatedDate == null && x.DeclinedDate == null &&
                    x.OrganisationTypeId == type.OrganisationTypeId);
        }

        public async Task<IEnumerable<NetworkRegisterRequest>> ListOpenNetworkRegisterRequestsAsync()
            //Show all that are "open"
            //(i.e. no action taken (accept, decline, create...))
            => await _networkRegisterRequestRepository.ListAsync(
                false,
                x => x.AcceptedDate == null && x.DeclinedDate == null && x.NetworkCreatedDate == null);

        public async Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync()
            //Show all that are "closed"
            //(i.e. declined or accepted)
            => await _organisationRegisterRequestRepository.ListAsync(
                false,
                x => x.DeclinedDate != null || x.AcceptedDate != null);

        public async Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalNetworkRegisterRequestsAsync()
            //Show all that are "closed"
            //(i.e. declined or accepted)
            => await _networkRegisterRequestRepository.ListAsync(
                false,
                x => x.DeclinedDate != null || x.AcceptedDate != null);

        public async Task<OrganisationRegisterRequest> GetBiobankRegisterRequestAsync(int requestId)
            => (await _organisationRegisterRequestRepository.ListAsync(
                    false,
                    x => x.OrganisationRegisterRequestId == requestId))
                .FirstOrDefault();

        public async Task<NetworkRegisterRequest> GetNetworkRegisterRequestAsync(int requestId)
            => (await _networkRegisterRequestRepository.ListAsync(
                    false,
                    x => x.NetworkRegisterRequestId == requestId))
                .FirstOrDefault();

        public async Task<Organisation> GetBiobankByExternalIdAsync(string externalId)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationExternalId == externalId && x.OrganisationTypeId == type.OrganisationTypeId,
                null,
                x => x.OrganisationAnnualStatistics)).FirstOrDefault();
        }

        public async Task<Organisation> GetBiobankByExternalIdForSearchResultsAsync(string externalId)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationExternalId == externalId && x.OrganisationTypeId == type.OrganisationTypeId,
                null,
                b => b.DiagnosisCapabilities,
                b => b.DiagnosisCapabilities.Select(c => c.SampleCollectionMode),
                b => b.DiagnosisCapabilities.Select(c => c.AssociatedData),
                b => b.DiagnosisCapabilities.Select(c => c.AssociatedData.Select(ad => ad.AssociatedDataType)),
                b => b.DiagnosisCapabilities.Select(c => c.AssociatedData.Select(ad => ad.AssociatedDataProcurementTimeframe)),
                b => b.OrganisationServiceOfferings.Select(o => o.ServiceOffering)
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<Organisation>> GetBiobanksByExternalIdsAsync(IList<string> biobankExternalIds)
            => await _organisationRepository.ListAsync(
                false,
                x => biobankExternalIds.Contains(x.OrganisationExternalId),
                null,
                x => x.OrganisationNetworks);

        public async Task<IEnumerable<Network>> GetNetworksByBiobankIdAsync(int organisationId)
        {
            var biobankNetworkIds =
                (await _networkOrganisationRepository.ListAsync(
                    false,
                    x => x.OrganisationId == organisationId)).Select(x => x.NetworkId);

            return await _networkRepository.ListAsync(
                false,
                x => biobankNetworkIds.Contains(x.NetworkId));
        }

        public async Task<bool> IsSnomedTermInUse(string id)
            => (await GetSnomedTermCollectionCapabilityCount(id) > 0);

        public async Task<bool> IsMaterialTypeInUse(int id)
            => (await GetMaterialTypeMaterialDetailCount(id) > 0);

        public async Task<bool> IsAssociatedDataTypeInUse(int id)
            => (await GetAssociatedDataTypeCollectionCapabilityCount(id) > 0);

        public async Task<bool> IsConsentRestrictionInUse(int id)
            => (await GetConsentRestrictionCollectionCount(id) > 0);

        public async Task<bool> IsAssociatedDataProcurementTimeFrameInUse (int id)
             => (await GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(id) > 0);

        public async Task<bool> IsCollectionTypeInUse(int id)
            => (await GetCollectionTypeCollectionCount(id) > 0);

        public async Task<bool> IsRegistrationReasonInUse(int id)
            => (await GetRegistrationReasonOrganisationCount(id) > 0);
        public async Task<bool> IsServiceOfferingInUse(int id)
            => (await GetServiceOfferingOrganisationCount(id) > 0);

        public async Task<bool> IsSexInUse(int id)
            => (await GetSexCount(id) > 0);

        public async Task<bool> IsHtaStatusInUse(int id)
            => (await GetHtaStatusCollectionCount(id) > 0);

        public async Task<bool> IsCountryInUse(int id)
            => (await GetCountryCountyOrganisationCount(id) > 0);

        public async Task<bool> IsAccessConditionInUse(int id)
            => (await GetAccessConditionsCount(id) > 0);

        public async Task<bool> IsCollectionStatusInUse(int id)
            => (await GetCollectionStatusCollectionCount(id) > 0);

        public async Task<IEnumerable<int>> GetAllSampleSetIdsAsync()
            => (await _sampleSetRepository.ListAsync()).Select(x => x.SampleSetId);

        public async Task<IEnumerable<int>> GetAllCapabilityIdsAsync()
            => (await _capabilityRepository.ListAsync()).Select(x => x.DiagnosisCapabilityId);

        public async Task<IEnumerable<CollectionSampleSet>> GetSampleSetsByIdsForIndexingAsync(
                IEnumerable<int> sampleSetIds)
            => await _sampleSetRepository.ListAsync(false,
                x => sampleSetIds.Contains(x.SampleSetId) && !x.Collection.Organisation.IsSuspended,
                null,
                x => x.Collection,
                x => x.Collection.SnomedTerm,
                x => x.Collection.Organisation,
                x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Collection.CollectionPoint,
                x => x.Collection.CollectionStatus,
                x => x.Collection.ConsentRestrictions,
                x => x.Collection.HtaStatus,
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

        public async Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(
                IEnumerable<int> capabilityIds)
            => await _capabilityRepository.ListAsync(false,
                x => capabilityIds.Contains(x.DiagnosisCapabilityId) && !x.Organisation.IsSuspended,
                null,
                x => x.Organisation,
                x => x.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Organisation.OrganisationServiceOfferings.Select(s => s.ServiceOffering),
                x => x.SnomedTerm,
                x => x.AssociatedData,
                x => x.SampleCollectionMode
            );

        public async Task<IEnumerable<CollectionSampleSet>> GetSampleSetsByIdsForIndexDeletionAsync(
                IEnumerable<int> sampleSetIds)
            => await _sampleSetRepository.ListAsync(false, x => sampleSetIds.Contains(x.SampleSetId), null,
                x => x.Collection,
                x => x.Collection.SnomedTerm,
                x => x.Collection.Organisation,
                x => x.Collection.Organisation.OrganisationNetworks.Select(on => on.Network),
                x => x.Collection.CollectionPoint,
                x => x.Collection.CollectionStatus,
                x => x.Collection.ConsentRestrictions,
                x => x.Collection.HtaStatus,
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
                x => x.SnomedTerm,
                x => x.AssociatedData,
                x => x.SampleCollectionMode
            );

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

        public async Task<Dictionary<int, string>> GetDescriptionsByCollectionIds(IEnumerable<int> collectionIds)
            => (await _collectionRepository.ListAsync(false,
                    x => collectionIds.Contains(x.CollectionId)))
                .Select(
                    x => new
                    {
                        id = x.CollectionId,
                        description = x.Description
                    })
                .ToDictionary(x => x.id, x => x.description);

        public async Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByOrganisationNameAsync(string name)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return (await _organisationRegisterRequestRepository.ListAsync(
                false,
                x => x.OrganisationName == name &&
                     x.OrganisationTypeId == type.OrganisationTypeId)).FirstOrDefault();
        }

        public async Task<bool> BiobankRegisterRequestExists(string name)
        {
            //We consider declined requests to not exist
            var type = await GetBiobankOrganisationTypeAsync();

            return (await _organisationRegisterRequestRepository.ListAsync(
                false,
                x => x.OrganisationName == name &&
                     x.DeclinedDate == null &&
                     x.OrganisationTypeId == type.OrganisationTypeId))
                .Any();
        }

        public async Task<bool> NetworkRegisterRequestExists(string name)
            //We consider declined requests to not exist
            => (await _networkRegisterRequestRepository.ListAsync(
                    false,
                    x => x.NetworkName == name &&
                         x.DeclinedDate == null))
                .Any();

        public async Task<int> GetSampleSetCountAsync()
            => await _sampleSetRepository.CountAsync();

        public async Task<int> GetCapabilityCountAsync()
            => await _capabilityRepository.CountAsync();

        public async Task<Organisation> GetBiobankByIdAsync(int biobankId)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId && x.OrganisationTypeId == type.OrganisationTypeId,
                null,
                x => x.OrganisationAnnualStatistics)).FirstOrDefault();
        }

        public async Task<Organisation> GetBiobankByIdForIndexingAsync(int biobankId)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId && x.OrganisationTypeId == type.OrganisationTypeId,
                null,
                b => b.Collections,
                b => b.Collections.Select(c => c.SampleSets),
                b => b.DiagnosisCapabilities,
                b => b.OrganisationServiceOfferings.Select(o => o.ServiceOffering)
                )).FirstOrDefault();
        }

        public async Task<Organisation> GetBiobankByNameAsync(string biobankName)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return (await _organisationRepository.ListAsync(
                false,
                x => x.OrganisationTypeId == type.OrganisationTypeId && x.Name == biobankName)).FirstOrDefault();
        }

        public async Task<NetworkRegisterRequest> GetNetworkRegisterRequestByUserEmailAsync(string email)
            //an email should only have one "active" Network register request at any one time
            //declined or created requests are no longer active!
            => (await _networkRegisterRequestRepository.ListAsync(
                false,
                x => x.UserEmail == email && x.DeclinedDate == null && x.NetworkCreatedDate == null)).FirstOrDefault();

        public async Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            //an email should only have one active Biobank register request at any one time
            //declined or created requests are no longer active!
            return (await _organisationRegisterRequestRepository.ListAsync(
                false,
                x =>
                    x.UserEmail == email && x.DeclinedDate == null && x.OrganisationCreatedDate == null &&
                    x.OrganisationTypeId == type.OrganisationTypeId))
                .FirstOrDefault();
        }

        public async Task<Collection> GetCollectionByIdAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.SnomedTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.CollectionPoint,
                x => x.ConsentRestrictions,
                x => x.HtaStatus,
                x => x.AssociatedData
            )).FirstOrDefault();

        // A method to search for a specific collection and get the value of it's FromApi boolean property.
        public async Task<bool> IsCollectionFromApi(int id)
        {
            Collection matchingCollection = (await _collectionRepository.ListAsync(false,
                 x => x.CollectionId == id,
                 null
             )).First();

            return matchingCollection.FromApi;
        }


        public async Task<Collection> GetCollectionByIdForIndexingAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.SnomedTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.CollectionPoint,
                x => x.ConsentRestrictions,
                x => x.HtaStatus,
                x => x.AssociatedData,
                x => x.AssociatedData.Select(y => y.AssociatedDataType),
                x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe),
                x => x.SampleSets
            )).FirstOrDefault();

        public async Task<Collection> GetCollectionWithSampleSetsByIdAsync(int id)
            => (await _collectionRepository.ListAsync(false,
                x => x.CollectionId == id,
                null,
                x => x.SnomedTerm,
                x => x.AccessCondition,
                x => x.CollectionType,
                x => x.CollectionStatus,
                x => x.CollectionPoint,
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
                x => x.SnomedTerm,
                x => x.SampleSets.Select(y => y.MaterialDetails));

            return collections;
        }

        public async Task<IEnumerable<Collection>> ListCollectionsAsync(int organisationId)
        {
            var collections = await _collectionRepository.ListAsync(
                false,
                x => x.OrganisationId == organisationId,
                null,
                x => x.SnomedTerm,
                x => x.SampleSets.Select(y => y.MaterialDetails));

            return collections;
        }

        public async Task<IEnumerable<SnomedTerm>> GetUsedSnomedTermsAsync()
        {
            var collections = await _collectionRepository.ListAsync(false);
            var uniqueSnomedTermsIds = collections.Select(x => x.SnomedTermId).Distinct();
            var uniqueSnomedTerms = await _snomedTermRepository.ListAsync(false, x => uniqueSnomedTermsIds.Contains(x.Id));

            return uniqueSnomedTerms;
        }

        public async Task<CollectionSampleSet> GetSampleSetByIdAsync(int id)
            => (await _sampleSetRepository.ListAsync(false, x => x.SampleSetId == id, null,
                x => x.Sex,
                x => x.AgeRange,
                x => x.DonorCount,
                x => x.MaterialDetails,
                x => x.MaterialDetails.Select(y => y.CollectionPercentage),
                x => x.MaterialDetails.Select(y => y.MacroscopicAssessment),
                x => x.MaterialDetails.Select(y => y.MaterialType),
                x => x.MaterialDetails.Select(y => y.StorageTemperature)
            )).FirstOrDefault();

        public async Task<CollectionSampleSet> GetSampleSetByIdForIndexingAsync(int id)
            => (await _sampleSetRepository.ListAsync(false, x => x.SampleSetId == id, null,
                x => x.Collection,
                x => x.Collection.SnomedTerm,
                x => x.Collection.Organisation,
                x => x.Collection.Organisation.OrganisationNetworks.Select(on => @on.Network),
                x => x.Collection.CollectionPoint,
                x => x.Collection.CollectionStatus,
                x => x.Collection.ConsentRestrictions,
                x => x.Collection.HtaStatus,
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

        public bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId)
            => _collectionRepository.List(
                false,
                x => x.OrganisationId == biobankId &&
                     x.CollectionId == collectionId).Any();

        public bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId)
            => _sampleSetRepository.List(
                false,
                x => x.Collection.OrganisationId == biobankId &&
                     x.SampleSetId == sampleSetId).Any();

        public async Task<DiagnosisCapability> GetCapabilityByIdAsync(int id)
            => (await _capabilityRepository.ListAsync(false,
                x => x.DiagnosisCapabilityId == id,
                null,
                x => x.SnomedTerm,
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
                x => x.SnomedTerm,
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
                x => x.SnomedTerm,
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

        public async Task<IEnumerable<AccessCondition>> ListAccessConditionsAsync()
            => await _accessConditionRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<IEnumerable<CollectionType>> ListCollectionTypesAsync()
            => await _collectionTypeRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));


        public async Task<IEnumerable<CollectionStatus>> ListCollectionStatusesAsync()
            => await _collectionStatusRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<IEnumerable<HtaStatus>> ListHtaStatusesAsync()
            => await _htaStatusRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<IEnumerable<Sex>> ListSexesAsync()
            => await _sexRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<IEnumerable<AssociatedDataType>> ListAssociatedDataTypesAsync()
            => await _associatedDataTypeRepository.ListAsync();

        public async Task<IEnumerable<ConsentRestriction>> ListConsentRestrictionsAsync()
            => await _collectionConsentRestrictionRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<IEnumerable<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeFrames()
            => await _associatedDataProcurementTimeFrameModelRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));
        public async Task<IEnumerable<MaterialType>> ListMaterialTypesAsync()
            => await _materialTypeRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        #region RefData: Collection Points
        public async Task<IEnumerable<CollectionPoint>> ListCollectionPointsAsync()
            => await _collectionPointRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<bool> ValidCollectionPointDescriptionAsync(string collectionPointDescription)
            => (await _collectionPointRepository.ListAsync(false, x => x.Description == collectionPointDescription)).Any();

        public async Task<bool> IsCollectionPointInUse(int id) 
            => (await GetCollectionPointUsageCount(id)) > 0;
        
        public async Task<int> GetCollectionPointUsageCount(int id)
            => (await _collectionRepository.ListAsync(false, x => x.CollectionPointId == id)).Count();
        #endregion

        #region RefData: Collection Percentages
        public async Task<IEnumerable<CollectionPercentage>> ListCollectionPercentagesAsync()
            => await _collectionPercentageRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<bool> ValidCollectionPercentageAsync(string collectionPercentageDescription) 
            => (await _collectionPercentageRepository.ListAsync(false, x => x.Description == collectionPercentageDescription)).Any();

        public async Task<bool> IsCollectionPercentageInUse(int id)
            => (await GetCollectionPercentageUsageCount(id)) > 0;

        public async Task<int> GetCollectionPercentageUsageCount(int id)
            => (await _materialDetailsRepository.ListAsync(false, x => x.CollectionPercentageId == id)).Count();
        #endregion

        #region RefData: Collection Type

        public async Task<int> GetCollectionTypeCollectionCount(int id)
             => (await _collectionRepository.ListAsync(false, x => x.CollectionTypeId == id)).Count();

        public async Task<bool> ValidCollectionTypeDescriptionAsync(string collectionTypeDescription)
            => (await _collectionTypeRepository.ListAsync(false, x => x.Description == collectionTypeDescription)).Any();

        public async Task<bool> ValidCollectionTypeDescriptionAsync(int collectionTypeId, string collectionTypeDescription)
            => (await _collectionTypeRepository.ListAsync(
                false,
                x => x.Description == collectionTypeDescription &&
                     x.CollectionTypeId != collectionTypeId)).Any();
        #endregion

        #region RefData: Macroscopic Assessments
        public async Task<IEnumerable<MacroscopicAssessment>> ListMacroscopicAssessmentsAsync()
            => await _macroscopicAssessmentRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<bool> ValidMacroscopicAssessmentAsync(string macroscopicAssessmentDescription)
            => (await _macroscopicAssessmentRepository.ListAsync(false, x => x.Description == macroscopicAssessmentDescription)).Any();

        public async Task<bool> IsMacroscopicAssessmentInUse(int id)
            => (await GetMacroscopicAssessmentUsageCount(id)) > 0;

        public async Task<int> GetMacroscopicAssessmentUsageCount(int id)
            => (await _materialDetailsRepository.ListAsync(false, x => x.MacroscopicAssessmentId == id)).Count();
        #endregion

        #region RefData: Annual Statistics
        public async Task<IEnumerable<AnnualStatistic>> ListAnnualStatisticsAsync()
            => await _annualStatisticRepository.ListAsync(false);

        public async Task<int> GetAnnualStatisticUsageCount(int id)
            => (await _organisationAnnualStatisticRepository.CountAsync(x => x.AnnualStatisticId == id));

        public async Task<bool> IsAnnualStatisticInUse(int id)
            => (await GetAnnualStatisticUsageCount(id)) > 0;

        public async Task<bool> ValidAnnualStatisticAsync(string annualStatisticDescription, int annualStatisticGroupId)
            => (await _annualStatisticRepository.ListAsync(false, x =>
                    x.Name == annualStatisticDescription && x.AnnualStatisticGroupId == annualStatisticGroupId
                )
            ).Any();

        #endregion

        #region RefData: Associated Data Type Groups

        public async Task<IEnumerable<AssociatedDataTypeGroup>> ListAssociatedDataTypeGroupsAsync(string wildcard = "")
            => await _associatedDataTypeGroupRepository.ListAsync(false, x => x.Description.Contains(wildcard));


        public async Task<int> GetAssociatedDataTypeGroupCount(int associatedDataTypeGroupId)
        => (await _associatedDataTypeRepository.ListAsync(
                   false,
                   x => x.AssociatedDataTypeGroupId == associatedDataTypeGroupId)).Count();

        public async Task<bool> IsAssociatedDataTypeGroupInUse(int associatedDataTypeGroupId)
            => (await GetAssociatedDataTypeGroupCount(associatedDataTypeGroupId) > 0);

        public async Task<bool> ValidAssociatedDataTypeGroupNameAsync(string associatedDataTypeGroupName)
            => (await _associatedDataTypeGroupRepository.ListAsync(false, x => x.Description == associatedDataTypeGroupName)).Any();

        public async Task<bool> ValidAssociatedDataTypeGroupNameAsync(int associatedDataTypeGroupId, string associatedDataTypeGroupName)
            => (await _associatedDataTypeGroupRepository.ListAsync(
                false,
                x => x.Description == associatedDataTypeGroupName &&
                     x.AssociatedDataTypeGroupId != associatedDataTypeGroupId)).Any();

        #endregion

        #region Donor Count
        public async Task<IEnumerable<DonorCount>> ListDonorCountsAsync(bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                return await _donorCountRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));
            }
            else
            {
                try
                {
                    return _cacheProvider.Retrieve<IEnumerable<DonorCount>>(CacheKeys.DonorCounts);
                }
                catch
                {
                    var donorCounts = await _donorCountRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));
                    _cacheProvider.Store<IEnumerable<DonorCount>>(CacheKeys.DonorCounts, donorCounts);

                    return donorCounts;
                }
            }
        }

        public async Task<bool> ValidDonorCountAsync(string donorCountDescription)
            => (await _donorCountRepository.ListAsync(false, x => x.Description == donorCountDescription)).Any();

        public async Task<bool> IsDonorCountInUse(int id)
            => (await GetDonorCountUsageCount(id)) > 0;

        public async Task<int> GetDonorCountUsageCount(int id)
            => (await _collectionSampleSetRepository.ListAsync(false, x => x.DonorCountId == id)).Count();
        #endregion

        #region Age Range
        public async Task<IEnumerable<AgeRange>> ListAgeRangesAsync()
            => await _ageRangeRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<int> GetAgeRangeUsageCount(int id)
            => (await _sampleSetRepository.ListAsync(false, x => x.AgeRangeId == id)).Count();

        public async Task<bool> IsAgeRangeInUse(int id)
            => (await GetAgeRangeUsageCount(id)) > 0;

        public async Task<bool> ValidAgeRangeAsync(string ageRangeDescription)
        {
            return (await _ageRangeRepository.ListAsync(false, x => x.Description == ageRangeDescription)).Any();
        }
        #endregion

        #region RefData: Sample Collection Mode
        public async Task<IEnumerable<SampleCollectionMode>> ListSampleCollectionModeAsync()
            => await _sampleCollectionModeRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<bool> ValidSampleCollectionModeAsync(string sampleCollectionModeDesc)
            => (await _sampleCollectionModeRepository.ListAsync(false, x => x.Description == sampleCollectionModeDesc)).Any();

        public async Task<bool> IsSampleCollectionModeInUse(int id)
            => (await GetSampleCollectionModeUsageCount(id)) > 0;

        public async Task<int> GetSampleCollectionModeUsageCount(int id)
            => (await _capabilityRepository.ListAsync(false, x => x.SampleCollectionModeId == id)).Count();
        #endregion

        #region RefData: Sop Status
        public async Task<IEnumerable<SopStatus>> ListSopStatusesAsync()
            => await _sopStatusRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<bool> ValidSopStatusAsync(string sopStatusDescription)
            => (await _sopStatusRepository.ListAsync(false, x => x.Description == sopStatusDescription)).Any();

        public async Task<bool> IsSopStatusInUse(int id)
            => (await GetSopStatusUsageCount(id)) > 0;

        public async Task<int> GetSopStatusUsageCount(int id)
            => (await _networkRepository.ListAsync(false, x => x.SopStatusId == id)).Count();
        #endregion



        public async Task<IEnumerable<StorageTemperature>> ListStorageTemperaturesAsync()
            => await _storageTemperatureRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

        public async Task<int> GetStorageTemperatureUsageCount(int id)
            => (await _materialDetailsRepository.ListAsync(false, x => x.StorageTemperatureId == id)).Count();

        public async Task<bool> IsStorageTemperatureInUse(int id)
            => (await GetStorageTemperatureUsageCount(id)) > 0;

        public async Task<bool> ValidStorageTemperatureAsync(string storageTemperature)
        {
            return (await _storageTemperatureRepository.ListAsync(false, x => x.Value == storageTemperature)).Any();
        }

        public async Task<IEnumerable<SnomedTerm>> ListSnomedTermsAsync(string wildcard = "")
            => await _snomedTermRepository.ListAsync(false, x => x.Description.Contains(wildcard));

        #region Site Config
        public IEnumerable<Config> ListSiteConfigs(string wildcard = "")
            => _siteConfigRepository.List(false, x => x.Key.Contains(wildcard));

        public async Task<IEnumerable<Config>> ListSiteConfigsAsync(string wildcard = "")
            => await _siteConfigRepository.ListAsync(false, x => x.Key.Contains(wildcard));

        public async Task<Config> GetSiteConfig(string key)
            => (await ListSiteConfigsAsync(key)).FirstOrDefault();

        public async Task<string> GetSiteConfigValue(string key, string defaultValue = "")
            => (await GetSiteConfig(key))?.Value ?? defaultValue;

        public async Task<bool> GetSiteConfigStatus(string siteConfigValue)
        {
            return (await _siteConfigRepository.ListAsync(false, x => x.Key == siteConfigValue && x.Value == "true")).Any();
        }



        #endregion

        public async Task<IEnumerable<SnomedTerm>> ListSearchableSnomedTermsAsync(SearchDocumentType type, string wildcard = "")
        {
            var searchableDiagnoses = _searchProvider.ListSnomedTerms(type, wildcard);

            return await _snomedTermRepository.ListAsync(false, x => searchableDiagnoses.Contains(x.Description));
        }

        public async Task<bool> ValidSnomedTermDescriptionAsync(string snomedTermDescription)
            => (await _snomedTermRepository.ListAsync(false, x => x.Description == snomedTermDescription)).Any();

        public async Task<bool> ValidSnomedTermDescriptionAsync(string snomedTermId, string snomedDescription)
            => (await _snomedTermRepository.ListAsync(
                false,
                x => x.Description == snomedDescription &&
                     x.Id != snomedTermId)).Any();

        public async Task<bool> ValidConsentRestrictionDescriptionAsync(string consentDescription)
    => (await _collectionConsentRestrictionRepository.ListAsync(false, x => x.Description == consentDescription)).Any();

        public async Task<bool> ValidConsentRestrictionDescriptionAsync(int consentId, string consentDescription)
            => (await _collectionConsentRestrictionRepository.ListAsync(
                false,
                x => x.Description == consentDescription &&
                     x.ConsentRestrictionId != consentId)).Any();

        public async Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(string procurementDescription)
        => (await _associatedDataProcurementTimeFrameModelRepository.ListAsync(false, x => x.Description == procurementDescription)).Any();

        public async Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(int procurementId, string procurementDescription)
            => (await _associatedDataProcurementTimeFrameModelRepository.ListAsync(
                false,
                x => x.Description == procurementDescription &&
                     x.AssociatedDataProcurementTimeframeId != procurementId)).Any();

        public async Task<bool> ValidHtaStatusDescriptionAsync(string htaStatusDescription)
            => (await _htaStatusRepository.ListAsync(false, x => x.Description == htaStatusDescription)).Any();

        public async Task<bool> ValidHtaStatusDescriptionAsync(int htaStatusId, string htaStatusDescription)
            => (await _htaStatusRepository.ListAsync(
                false,
                x => x.Description == htaStatusDescription &&
                     x.HtaStatusId != htaStatusId)).Any();

        public async Task<bool> ValidServiceOfferingName(string offeringName)
            => (await _serviceOfferingRepository.ListAsync(false, x => x.Name == offeringName)).Any();

        public async Task<bool> ValidServiceOfferingName(int offeringId, string offeringName)
            => (await _serviceOfferingRepository.ListAsync(
                false,
                x => x.Name == offeringName &&
                     x.ServiceId != offeringId)).Any();
        public async Task<bool> ValidAssociatedDataTypeDescriptionAsync(string associatedDataTypeDescription)
    => (await _associatedDataTypeRepository.ListAsync(false, x => x.Description == associatedDataTypeDescription)).Any();
        public async Task<bool> ValidAssociatedDataTypeDescriptionAsync(int associatedDataTypeId, string associatedDataTypeDescription)
            => (await _associatedDataTypeRepository.ListAsync(
                false,
                x => x.Description == associatedDataTypeDescription &&
                     x.AssociatedDataTypeId != associatedDataTypeId)).Any();

        public async Task<bool> ValidRegistrationReasonDescriptionAsync(string reasonDescription)
            => (await _registrationReasonRepository.ListAsync(false, x => x.Description == reasonDescription)).Any();
        public async Task<bool> ValidRegistrationReasonDescriptionAsync(int reasonId, string reasonDescription)
            => (await _registrationReasonRepository.ListAsync(
                false,
                x => x.Description == reasonDescription &&
                     x.RegistrationReasonId != reasonId)).Any();

        public async Task<bool> ValidCountryNameAsync(string countryName)
      => (await _countryRepository.ListAsync(false, x => x.Name == countryName)).Any();

        public async Task<bool> ValidCountryNameAsync(int countryId, string countryName)
            => (await _countryRepository.ListAsync(
                false,
                x => x.Name == countryName &&
                     x.CountryId != countryId)).Any();

        public async Task<bool> ValidCollectionStatusDescriptionAsync(string collectionStatusDescription)
        => (await _collectionStatusRepository.ListAsync(false, x => x.Description == collectionStatusDescription)).Any();

        public async Task<bool> ValidCollectionStatusDescriptionAsync(int collectionStatusId, string collectionStatusDescription)
            => (await _collectionStatusRepository.ListAsync(
                false,
                x => x.Description == collectionStatusDescription &&
                     x.CollectionStatusId != collectionStatusId)).Any();

        public async Task<SnomedTerm> GetSnomedTermByDescription(string description)
            => (await _snomedTermRepository.ListAsync(false, x => x.Description == description)).Single();

        public async Task<int> GetSnomedTermCollectionCapabilityCount(string id)
        => (await _collectionRepository.ListAsync(
                   false,
                   x => x.SnomedTermId == id)).Count() + (await _capabilityRepository.ListAsync(
                   false,
                   x => x.SnomedTermId == id)).Count();

        public async Task<int> GetMaterialTypeMaterialDetailCount(int id)
      => (await _materialDetailRepository.ListAsync(
                 false,
                 x => x.MaterialTypeId == id)).Count();

        public async Task<bool> ValidMaterialTypeDescriptionAsync(string materialTypeDescription)
            => (await _materialTypeRepository.ListAsync(false, x => x.Value == materialTypeDescription)).Any();

        public async Task<bool> ValidMaterialTypeDescriptionAsync(int materialTypeId, string materialTypeDescription)
            => (await _materialTypeRepository.ListAsync(
                false,
                x => x.Value == materialTypeDescription &&
                     x.Id != materialTypeId)).Any();

        public async Task<int> GetConsentRestrictionCollectionCount(int id)
        {
            var restrictions = (await _collectionConsentRestrictionRepository.ListAsync(false, x => x.ConsentRestrictionId == id, null, x => x.Collections)).SingleOrDefault();
            if (restrictions is null)
            {
                throw new KeyNotFoundException();
            }
            else
            {
                return restrictions.Collections.Count();
            }
        }


        public async Task<int> GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(int id)
            => (await _collectionAssociatedDataRepository.ListAsync(
           false,
           x => x.AssociatedDataProcurementTimeframeId == id)).Count() + (await _capabilityAssociatedDataRepository.ListAsync(
           false,
           x => x.AssociatedDataProcurementTimeframeId == id)).Count();

        public async Task<int> GetCollectionStatusCollectionCount(int id)
            => (await _collectionRepository.ListAsync(false, x => x.CollectionStatusId == id)).Count();

        public async Task<int> GetServiceOfferingOrganisationCount(int id)
            => (await _organisationServiceOfferingRepository.ListAsync(
            false,
             x => x.ServiceId == id)).Count();

        public async Task<int> GetHtaStatusCollectionCount(int id)
            => (await _collectionRepository.ListAsync(false, x => x.HtaStatusId == id)).Count();
        public async Task<int> GetRegistrationReasonOrganisationCount(int id)
                    => (await _organisationRegistrationReasonRepository.ListAsync(false, x => x.RegistrationReasonId == id)).Count();

        public async Task<int> GetSexCount(int id)
        => (await _sampleSetRepository.ListAsync(
                   false,
                   x => x.SexId == id)).Count();

        public async Task<bool> ValidSexDescriptionAsync(string sexDescription)
            => (await _sexRepository.ListAsync(false, x => x.Value == sexDescription)).Any();

        public async Task<bool> ValidSexDescriptionAsync(int sexId, string sexDescription)
            => (await _sexRepository.ListAsync(
                false,
                x => x.Value == sexDescription &&
                     x.Id != sexId)).Any();

        public async Task<int> GetAccessConditionsCount(int id)
         => (await _collectionRepository.ListAsync(
                    false,
                    x => x.AccessConditionId == id)).Count();

        public async Task<bool> ValidAccessConditionDescriptionAsync(string accessConditionsDescription)
            => (await _accessConditionRepository.ListAsync(false, x => x.Description == accessConditionsDescription)).Any();

        public async Task<bool> ValidAccessConditionDescriptionAsync(int accessConditionsId, string accessConditionsDescription)
            => (await _accessConditionRepository.ListAsync(
                false,
                x => x.Description == accessConditionsDescription &&
                     x.AccessConditionId != accessConditionsId)).Any();

        public async Task<int> GetAssociatedDataTypeCollectionCapabilityCount(int id)
        => (await _collectionAssociatedDataRepository.ListAsync(
                   false,
                   x => x.AssociatedDataTypeId == id)).Count() + (await _capabilityAssociatedDataRepository.ListAsync(
                   false,
                   x => x.AssociatedDataTypeId == id)).Count();

        public async Task<int> GetCountryCountyOrganisationCount(int id)
            => (await _countyRepository.ListAsync(
                false,
                x => x.CountryId == id)).Count() + (await _organisationRepository.ListAsync(
                false,
                x => x.CountryId == id)).Count();

        public async Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId)
            => await _organisationServiceOfferingRepository.ListAsync(
                false,
                x => x.OrganisationId == biobankId,
                null,
                x => x.ServiceOffering);

        public async Task<IEnumerable<ServiceOffering>> ListServiceOfferingsAsync()
            => await _serviceOfferingRepository.ListAsync(false, null, x => x.OrderBy(y => y.SortOrder));

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
            // TODO remove the generic repo when upgrading to netcore, as it doesn't support groupby fully
            var admins = await _organisationUserRepository.ListAsync(false);
            var adminIds = admins.GroupBy(a => a.OrganisationUserId)
                .Select(a => a.FirstOrDefault(ai => ai.OrganisationId == biobankId))
                .Select(ou => ou?.OrganisationUserId);

            return await _userManager.Users.Where(x => adminIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<Funder> GetFunderByIdAsync(int id)
            => await _funderRepository.GetByIdAsync(id);

        public List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId)
        {
            var userOrgIds =  _organisationUserRepository.List(
                    false,
                    x => x.OrganisationUserId == userId)
                .Select(x => x.OrganisationId)
                .ToList();

            var userOrganisations = _organisationRepository.List(
                    false,
                    x => userOrgIds.Contains(x.OrganisationId))
                .ToList();

            return userOrganisations.Select(o => new KeyValuePair<int, string>(o.OrganisationId, o.Name)).ToList();
        }

        public List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId)
        {
            var userEmail = _userManager.Users.FirstOrDefault(u => u.Id == userId)?.Email;

            return _organisationRegisterRequestRepository.List(
                    false,
                    x => x.UserEmail == userEmail
                         && x.AcceptedDate != null
                         && x.OrganisationCreatedDate == null)
                .Select(r => new KeyValuePair<int, string>(r.OrganisationRegisterRequestId, r.OrganisationName))
                .ToList();
        }

        public List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId)
        {
            var userNetworkIds = _networkUserRepository.List(
                    false,
                    x => x.NetworkUserId == userId)
                .Select(x => x.NetworkId)
                .ToList();

            var userNetworks = _networkRepository.List(
                false,
                x => userNetworkIds.Contains(x.NetworkId)).ToList();

            return userNetworks.Select(n => new KeyValuePair<int, string>(n.NetworkId, n.Name)).ToList();
        }

        public List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId)
        {
            var userEmail = _userManager.Users.FirstOrDefault(u => u.Id == userId)?.Email;

            return _networkRegisterRequestRepository.List(
                    false,
                    x => x.UserEmail == userEmail
                         && x.AcceptedDate != null
                         && x.NetworkCreatedDate == null)
                .Select(r => new KeyValuePair<int, string>(r.NetworkRegisterRequestId, r.NetworkName))
                .ToList();
        }
        
        public async Task<IEnumerable<AnnualStatisticGroup>> GetAnnualStatisticGroupsAsync()
            =>  await _annualStatisticGroupRepository.ListAsync(false, null, null, asg => asg.AnnualStatistics);

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds)
            => await _networkOrganisationRepository.ListAsync(
                false,
                on => organisationIds.Contains(on.OrganisationId),
                null,
                on => on.Network);

        public async Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers)
            => await _organisationRepository.ListAsync(false, o => o.AnonymousIdentifier.HasValue && biobankAnonymousIdentifiers.Contains(o.AnonymousIdentifier.Value));

        public async Task<IEnumerable<RegistrationReason>> ListRegistrationReasonsAsync()
            => await _registrationReasonRepository.ListAsync();

        public async Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId)
            => await _organisationRegistrationReasonRepository.ListAsync(
                false,
                x => x.OrganisationId == organisationId,
                null,
                x => x.RegistrationReason);

        public async Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
            => await _publicationRepository.ListAsync(true, x => x.OrganisationId == biobankId);

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId)
            => await _organisationNetworkRepository.ListAsync(false, x => x.OrganisationId == biobankId);

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId)
            => await _organisationNetworkRepository.ListAsync(false, x => x.OrganisationId == biobankId && x.NetworkId == networkId);

        public async Task<IEnumerable<Publication>> GetOrganisationPublicationsAsync(Organisation organisation)
            => await ListOrganisationPublications(organisation.OrganisationId);

        public async Task<IEnumerable<Publication>> GetAcceptedOrganisationPublicationsAsync(Organisation organisation)
            => (await GetOrganisationPublicationsAsync(organisation)).Where(x => x.Accepted == true);

        public List<Organisation> GetOrganisations() => _organisationRepository.List(false, x => x.IsSuspended == false, x => x.OrderBy(c => c.Name)).ToList();

        public async Task<IEnumerable<AnnualStatisticGroup>> ListAnnualStatisticGroupsAsync(string wildcard = "")
        => await _annualStatisticGroupRepository.ListAsync(false, x => x.Name.Contains(wildcard));

        public async Task<bool> ValidAnnualStatisticGroupNameAsync(string annualStatisticGroupName)
            => (await _annualStatisticGroupRepository.ListAsync(false, x => x.Name == annualStatisticGroupName)).Any();

        public async Task<bool> ValidAnnualStatisticGroupNameAsync(int annualStatisticGroupId, string annualStatisticGroupName)
            => (await _annualStatisticGroupRepository.ListAsync(
                false,
                x => x.Name == annualStatisticGroupName &&
                     x.AnnualStatisticGroupId != annualStatisticGroupId)).Any();

        public async Task<AnnualStatisticGroup> GetAnnualStatisticGroupByName(string name)
            => (await _annualStatisticGroupRepository.ListAsync(false, x => x.Name == name)).Single();

        public async Task<int> GetAnnualStatisticAnnualStatisticGroupCount(int annualStatisticGroupId)
        => (await _annualStatisticRepository.ListAsync(
                   false,
                   x => x.AnnualStatisticGroupId == annualStatisticGroupId)).Count();

        public async Task<bool> IsAnnualStatisticGroupInUse(int annualStatisticGroupId)
            => (await GetAnnualStatisticAnnualStatisticGroupCount(annualStatisticGroupId) > 0);

    }
}