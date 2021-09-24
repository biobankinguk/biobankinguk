using Biobanks.Directory.Data.Caching;
using Biobanks.Directory.Data.Repositories;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Constants;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Services
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

        public async Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync()
            //Show all that are accepted but not yet created
            //filter by no created date, but an existing accepted date
            => await _organisationRegisterRequestRepository.ListAsync(
                false,
                x => x.AcceptedDate != null && x.DeclinedDate == null && x.OrganisationCreatedDate == null);

        public async Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedNetworkRegisterRequestAsync()
            //Show all that are accepted but not yet created
            //filter by no created date, but an existing accepted date
            => await _networkRegisterRequestRepository.ListAsync(
                false,
                x => x.AcceptedDate != null && x.DeclinedDate == null && x.NetworkCreatedDate == null);


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
        .Where(x=> x.DisplayOnDirectory || !onlyDisplayable)
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
                x => x.OntologyTerm.Value == ontologyTerm)).Select(x=>x.CollectionId);

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

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds)
            => await _networkOrganisationRepository.ListAsync(
                false,
                on => organisationIds.Contains(on.OrganisationId),
                null,
                on => on.Network);

        public async Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers)
            => await _organisationRepository.ListAsync(false, o => o.AnonymousIdentifier.HasValue && biobankAnonymousIdentifiers.Contains(o.AnonymousIdentifier.Value));

        public async Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId)
            => await _organisationRegistrationReasonRepository.ListAsync(
                false,
                x => x.OrganisationId == organisationId,
                null,
                x => x.RegistrationReason);

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId)
            => await _organisationNetworkRepository.ListAsync(false, x => x.OrganisationId == biobankId);

        public async Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId)
            => await _organisationNetworkRepository.ListAsync(false, x => x.OrganisationId == biobankId && x.NetworkId == networkId);

        public List<Organisation> GetOrganisations() => _organisationRepository.List(false, x => x.IsSuspended == false, x => x.OrderBy(c => c.Name)).ToList();

        public async Task<bool> OrganisationIncludesPublications(int biobankId)
            => (!(await GetBiobankByIdAsync(biobankId)).ExcludePublications);

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
        
        public async Task<bool> IsBiobankAnApiClient(int biobankId)
            => ((await GetBiobankByIdAsync(biobankId)).ApiClients.Any());

    }
}