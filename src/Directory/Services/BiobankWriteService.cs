using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Directory.Entity.Data;
using Directory.Data.Repositories;
using Directory.Services.Dto;
using Directory.Services.Contracts;
using System.IO;
using AutoMapper;
using Nest;
using AutoMapper.Internal;
using System.Runtime.InteropServices;
using Directory.Services.Extensions;
using Microsoft.Owin.Security.Provider;

namespace Directory.Services
{
    public class BiobankWriteService : IBiobankWriteService
    {
        #region Properties and ctor

        private readonly IBiobankReadService _biobankReadService;

        private readonly IBlobStorageProvider _blobStorageProvider;

        private readonly IGenericEFRepository<Diagnosis> _diagnosisRepository;
        private readonly IGenericEFRepository<AgeRange> _ageRangeRepository;
        private readonly IGenericEFRepository<CollectionPoint> _collectionPointRepository;
        private readonly IGenericEFRepository<CollectionPercentage> _collectionPercentageRepository;
        private readonly IGenericEFRepository<DonorCount> _donorCountRepository;
        private readonly IGenericEFRepository<SampleCollectionMode> _sampleCollectionModeRepository;
        private readonly IGenericEFRepository<MacroscopicAssessment> _macroscopicAssessmentRepository;
        private readonly IGenericEFRepository<PreservationType> _preservationTypeRepository;
        private readonly IGenericEFRepository<MaterialType> _materialTypeRepository;
        private readonly IGenericEFRepository<Sex> _sexRepository;
        private readonly IGenericEFRepository<SopStatus> _sopStatusRepository;
        private readonly IGenericEFRepository<AnnualStatistic> _annualStatisticRepository;
        private readonly IGenericEFRepository<AnnualStatisticGroup> _annualStatisticGroupRepository;
        private readonly IGenericEFRepository<AssociatedDataType> _associatedDataTypeRepository;
        private readonly IGenericEFRepository<AssociatedDataTypeGroup> _associatedDataTypeGroupRepository;
        private readonly IGenericEFRepository<AccessCondition> _accessConditionRepository;
        private readonly IGenericEFRepository<ConsentRestriction> _consentRestrictionRepository;
        private readonly IGenericEFRepository<Country> _countryRepository;
        private readonly IGenericEFRepository<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameRepository;
        private readonly IGenericEFRepository<CollectionType> _collectionTypeRepository;
        private readonly IGenericEFRepository<County> _countyRepository;
        private readonly IGenericEFRepository<CollectionStatus> _collectionStatusRepository;

        private readonly IGenericEFRepository<Collection> _collectionRepository;
        private readonly IGenericEFRepository<DiagnosisCapability> _capabilityRepository;
        private readonly IGenericEFRepository<CollectionSampleSet> _sampleSetRepository;

        private readonly IGenericEFRepository<Network> _networkRepository;
        private readonly IGenericEFRepository<NetworkUser> _networkUserRepository;
        private readonly IGenericEFRepository<NetworkRegisterRequest> _networkRegisterRequestRepository;
        private readonly IGenericEFRepository<OrganisationNetwork> _networkOrganisationRepository;

        private readonly IGenericEFRepository<Organisation> _organisationRepository;
        private readonly IGenericEFRepository<OrganisationAnnualStatistic> _organisationAnnualStatisticRepository;
        private readonly IGenericEFRepository<OrganisationRegisterRequest> _organisationRegisterRequestRepository;
        private readonly IGenericEFRepository<OrganisationRegistrationReason> _organisationRegistrationReasonRepository;
        private readonly IGenericEFRepository<OrganisationUser> _organisationUserRepository;
        private readonly IGenericEFRepository<OrganisationType> _organisationTypeRepository;
        private readonly IGenericEFRepository<OrganisationServiceOffering> _organisationServiceOfferingRepository;
        private readonly IGenericEFRepository<RegistrationReason> _registrationReasonRepository;
        private readonly IGenericEFRepository<ServiceOffering> _serviceOfferingRepository;
        private readonly IGenericEFRepository<HtaStatus> _htaStatusRepository;

        private readonly IGenericEFRepository<Publication> _publicationRespository;

        private readonly IGenericEFRepository<Funder> _funderRepository;

        private readonly IGenericEFRepository<Config> _siteConfigRepository;

        private readonly IBiobankIndexService _indexService;

        private readonly IMapper _mapper;

        public BiobankWriteService(
            IBiobankReadService biobankReadService,
            IBlobStorageProvider blobStorageProvider,
            IGenericEFRepository<Diagnosis> diagnosisRepository,
            IGenericEFRepository<MaterialType> materialTypeRepository,
            IGenericEFRepository<Sex> sexRepository,
            IGenericEFRepository<AnnualStatistic> annualStatisticRepository,
            IGenericEFRepository<AnnualStatisticGroup> annualStatisticGroupRepository,
            IGenericEFRepository<AssociatedDataType> associatedDataTypeRepository,
            IGenericEFRepository<AssociatedDataTypeGroup> associatedDataTypeGroupRepository,
            IGenericEFRepository<CollectionPercentage> collectionPercentageRepository,
            IGenericEFRepository<DonorCount> donorCountRepository,
            IGenericEFRepository<CollectionPoint> collectionPointRepository,
            IGenericEFRepository<CollectionType> collectionTypeRepository,
            IGenericEFRepository<CollectionStatus> collectionStatusRepository,
            IGenericEFRepository<AgeRange> ageRangeRepository,
            IGenericEFRepository<MacroscopicAssessment> macroscopicAssessmentRepository,
            IGenericEFRepository<SampleCollectionMode> sampleCollectionModeRepository,
            IGenericEFRepository<PreservationType> preservationTypeRepository,
            IGenericEFRepository<AccessCondition> accessConditionRepository,
            IGenericEFRepository<SopStatus> sopStatusRepository,
            IGenericEFRepository<ConsentRestriction> consentRestrictionRepository,
            IGenericEFRepository<Country> countryRepository,
            IGenericEFRepository<County> countyRepository,
            IGenericEFRepository<Collection> collectionRepository,
            IGenericEFRepository<DiagnosisCapability> capabilityRepository,
            IGenericEFRepository<CollectionSampleSet> sampleSetRepository,
            IGenericEFRepository<Network> networkRepository,
            IGenericEFRepository<NetworkUser> networkUserRepository,
            IGenericEFRepository<NetworkRegisterRequest> networkRegisterRequestRepository,
            IGenericEFRepository<OrganisationNetwork> networkOrganisationRepository,
            IGenericEFRepository<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameRepository,

        IGenericEFRepository<Organisation> organisationRepository,
            IGenericEFRepository<OrganisationAnnualStatistic> organisationAnnualStatisticRepository,
            IGenericEFRepository<OrganisationRegisterRequest> organisationRegisterRequestRepository,
            IGenericEFRepository<OrganisationRegistrationReason> organisationRegistrationReasonRepository,
            IGenericEFRepository<OrganisationUser> organisationUserRepository,
            IGenericEFRepository<OrganisationType> organisationTypeRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<RegistrationReason> registrationReasonRepository,
            IGenericEFRepository<ServiceOffering> serviceOfferingRepository,
            IGenericEFRepository<HtaStatus> htaStatusRepository,

            IGenericEFRepository<Publication> publicationRepository,

            IGenericEFRepository<Config> siteConfigRepository,

            IBiobankIndexService indexService,
            IMapper mapper,

            IGenericEFRepository<Funder> funderRepository)
        {
            _biobankReadService = biobankReadService;

            _blobStorageProvider = blobStorageProvider;

            _diagnosisRepository = diagnosisRepository;
            _collectionPercentageRepository = collectionPercentageRepository;
            _donorCountRepository = donorCountRepository;
            _collectionPointRepository = collectionPointRepository;
            _ageRangeRepository = ageRangeRepository;
            _macroscopicAssessmentRepository = macroscopicAssessmentRepository;
            _preservationTypeRepository = preservationTypeRepository;
            _sopStatusRepository = sopStatusRepository;
            _sampleCollectionModeRepository = sampleCollectionModeRepository;
            _associatedDataTypeRepository = associatedDataTypeRepository;
            _associatedDataTypeGroupRepository = associatedDataTypeGroupRepository;
            _annualStatisticRepository = annualStatisticRepository;
            _annualStatisticGroupRepository = annualStatisticGroupRepository;
            _accessConditionRepository = accessConditionRepository;
            _materialTypeRepository = materialTypeRepository;
            _sexRepository = sexRepository;
            _consentRestrictionRepository = consentRestrictionRepository;
            _countryRepository = countryRepository;
            _countyRepository = countyRepository;
            _collectionStatusRepository = collectionStatusRepository;
            _collectionTypeRepository = collectionTypeRepository;

            _collectionRepository = collectionRepository;
            _capabilityRepository = capabilityRepository;
            _sampleSetRepository = sampleSetRepository;

            _networkRepository = networkRepository;
            _networkUserRepository = networkUserRepository;
            _networkRegisterRequestRepository = networkRegisterRequestRepository;
            _networkOrganisationRepository = networkOrganisationRepository;

            _organisationRepository = organisationRepository;
            _organisationAnnualStatisticRepository = organisationAnnualStatisticRepository;
            _organisationRegisterRequestRepository = organisationRegisterRequestRepository;
            _organisationRegistrationReasonRepository = organisationRegistrationReasonRepository;
            _organisationUserRepository = organisationUserRepository;
            _organisationTypeRepository = organisationTypeRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _registrationReasonRepository = registrationReasonRepository;
            _serviceOfferingRepository = serviceOfferingRepository;
            _associatedDataProcurementTimeFrameRepository = associatedDataProcurementTimeFrameRepository;
            _htaStatusRepository = htaStatusRepository;

            _publicationRespository = publicationRepository;

            _siteConfigRepository = siteConfigRepository;

            _indexService = indexService;

            _mapper = mapper;
            _funderRepository = funderRepository;
        }

        #endregion

        public async Task<Collection> AddCollectionAsync(
            Collection collection,
            string diagnosisDescription,
            IEnumerable<CollectionAssociatedData> associatedData,
            IEnumerable<int> consentRestrictionIds)
        {
            var diagnosis = await _biobankReadService.GetDiagnosisByDescription(diagnosisDescription);
            var consentRestrictions = (await _consentRestrictionRepository.ListAsync(true,
                        x => consentRestrictionIds.Contains(x.ConsentRestrictionId))).ToList();

            collection.DiagnosisId = diagnosis.DiagnosisId;
            collection.LastUpdated = DateTime.Now;
            collection.AssociatedData = associatedData.ToList();
            collection.ConsentRestrictions = consentRestrictions;

            _collectionRepository.Insert(collection);

            await _collectionRepository.SaveChangesAsync();

            return collection;
        }

        public async Task UpdateCollectionAsync(
            Collection collection,
            string diagnosisDescription,
            IEnumerable<CollectionAssociatedData> associatedData,
            IEnumerable<int> consentRestrictionIds)
        {
            var existingCollection = (await _collectionRepository.ListAsync(true,
                x => x.CollectionId == collection.CollectionId,
                null,
                x => x.AssociatedData,
                x => x.ConsentRestrictions)).First();

            existingCollection.AssociatedData.Clear();
            existingCollection.ConsentRestrictions.Clear();

            var diagnosis = await _biobankReadService.GetDiagnosisByDescription(diagnosisDescription);
            var consentRestrictions = (await _consentRestrictionRepository.ListAsync(true,
                        x => consentRestrictionIds.Contains(x.ConsentRestrictionId))).ToList();

            existingCollection.DiagnosisId = diagnosis.DiagnosisId;
            existingCollection.Title = collection.Title;
            existingCollection.Description = collection.Description;
            existingCollection.StartDate = collection.StartDate;
            existingCollection.HtaStatusId = collection.HtaStatusId;
            existingCollection.AccessConditionId = collection.AccessConditionId;
            existingCollection.CollectionTypeId = collection.CollectionTypeId;
            existingCollection.CollectionStatusId = collection.CollectionStatusId;
            existingCollection.CollectionPointId = collection.CollectionPointId;
            existingCollection.LastUpdated = DateTime.Now;

            existingCollection.AssociatedData = associatedData.ToList();
            existingCollection.ConsentRestrictions = consentRestrictions;

            await _collectionRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCollectionBiobankSuspendedAsync(collection.CollectionId))
                await _indexService.UpdateCollectionDetails(collection.CollectionId);
        }

        public async Task<bool> DeleteCollectionAsync(int id)
        {
            var collection = (await _collectionRepository
                .ListAsync(true, x => x.CollectionId == id, null, x => x.SampleSets))
                .First();

            if (collection.SampleSets.Any()) return false;

            await _collectionRepository.DeleteWhereAsync(x => x.CollectionId == id);

            await _collectionRepository.SaveChangesAsync();

            return true;
        }

        public async Task AddSampleSetAsync(CollectionSampleSet sampleSet)
        {
            _sampleSetRepository.Insert(sampleSet);

            await _sampleSetRepository.SaveChangesAsync();

            var collection = await _collectionRepository.GetByIdAsync(sampleSet.CollectionId);

            collection.LastUpdated = DateTime.Now;

            await _collectionRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCollectionBiobankSuspendedAsync(sampleSet.CollectionId))
                await _indexService.IndexSampleSet(sampleSet.SampleSetId);
        }

        public async Task UpdateSampleSetAsync(CollectionSampleSet sampleSet)
        {
            var existingSampleSet = (await _sampleSetRepository.ListAsync(
                true, x => x.SampleSetId == sampleSet.SampleSetId, null,
                x => x.Collection, x => x.MaterialDetails)).First();

            existingSampleSet.MaterialDetails.Clear();

            existingSampleSet.SexId = sampleSet.SexId;
            existingSampleSet.AgeRangeId = sampleSet.AgeRangeId;
            existingSampleSet.DonorCountId = sampleSet.DonorCountId;
            existingSampleSet.MaterialDetails = sampleSet.MaterialDetails;

            existingSampleSet.Collection.LastUpdated = DateTime.Now;

            await _sampleSetRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCollectionBiobankSuspendedAsync(existingSampleSet.CollectionId))
                await _indexService.UpdateSampleSetDetails(sampleSet.SampleSetId);
        }

        public async Task DeleteSampleSetAsync(int id)
        {
            //we need to check if the sampleset belongs to a suspended bb, BEFORE we delete the sampleset
            var suspended = await _biobankReadService.IsSampleSetBiobankSuspendedAsync(id);

            await _sampleSetRepository.DeleteWhereAsync(x => x.SampleSetId == id);

            await _sampleSetRepository.SaveChangesAsync();

            if (!suspended)
                _indexService.DeleteSampleSet(id);
        }

        public async Task AddCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
            var diagnosis = await _biobankReadService.GetDiagnosisByDescription(capabilityDTO.Diagnosis);

            var capability = new DiagnosisCapability
            {
                OrganisationId = capabilityDTO.OrganisationId,
                DiagnosisId = diagnosis.DiagnosisId,
                AnnualDonorExpectation = capabilityDTO.AnnualDonorExpectation.Value,
                AssociatedData = associatedData.ToList(),
                SampleCollectionModeId = capabilityDTO.SampleCollectionModeId,
                LastUpdated = DateTime.Now
            };

            _capabilityRepository.Insert(capability);

            await _capabilityRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCapabilityBiobankSuspendedAsync(capability.DiagnosisCapabilityId))
                await _indexService.IndexCapability(capability.DiagnosisCapabilityId);
        }

        public async Task UpdateCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
            var existingCapability = (await _capabilityRepository.ListAsync(true,
                x => x.DiagnosisCapabilityId == capabilityDTO.Id,
                null,
                x => x.AssociatedData)).First();

            existingCapability.AssociatedData.Clear();

            var diagnosis = await _biobankReadService.GetDiagnosisByDescription(capabilityDTO.Diagnosis);

            existingCapability.DiagnosisId = diagnosis.DiagnosisId;
            existingCapability.AnnualDonorExpectation = capabilityDTO.AnnualDonorExpectation.Value;
            existingCapability.SampleCollectionModeId = capabilityDTO.SampleCollectionModeId;
            existingCapability.LastUpdated = DateTime.Now;

            existingCapability.AssociatedData = associatedData.ToList();

            await _capabilityRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCapabilityBiobankSuspendedAsync(existingCapability.DiagnosisCapabilityId))
                await _indexService.UpdateCapabilityDetails(existingCapability.DiagnosisCapabilityId);
        }

        public async Task DeleteCapabilityAsync(int id)
        {
            await _capabilityRepository.DeleteWhereAsync(x => x.DiagnosisCapabilityId == id);

            await _capabilityRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsCapabilityBiobankSuspendedAsync(id))
                _indexService.DeleteCapability(id);
        }

        public async Task<Organisation> CreateBiobankAsync(OrganisationDTO biobankDto)
        {
            //Partially create External ID if necessary
            //we can't add this biobank's OrganisationId until it's created, so we'll defer that bit for now
            //but we have to write something as external id is not nullable
            var biobank = _mapper.Map<Organisation>(biobankDto);

            var type = (await _organisationTypeRepository.ListAsync(
                false,
                x => x.Description == "Biobank")).FirstOrDefault();

            //Only UK at this time, but this allows for ISO3 country codes in future
            biobank.OrganisationExternalId = "GBR-" + type.OrganisationTypeId + "-";
            biobank.OrganisationTypeId = type.OrganisationTypeId;

            // create new anonymous id
            biobank.AnonymousIdentifier = Guid.NewGuid();

            biobank.LastUpdated = DateTime.Now;

            _organisationRepository.Insert(biobank);
            var result = await _organisationRepository.SaveChangesAsync();

            if (result != 1) throw new DataException(); //shouldn't be doing anything other than inserting one organisation!

            //Now the organisation exists, update its external id, to include its internal id
            biobank.OrganisationExternalId = biobank.OrganisationExternalId + biobank.OrganisationId;
            _organisationRepository.Update(biobank);
            result = await _organisationRepository.SaveChangesAsync();
            if (result != 1) throw new DataException(); //shouldn't be doing anything other than updating one organisation!

            return biobank;
        }

        public async Task<Organisation> UpdateBiobankAsync(OrganisationDTO biobankDto)
        {
            //Get the bb entity and update it
            var biobank = await _organisationRepository.GetByIdAsync(biobankDto.OrganisationId);

            _mapper.Map(biobankDto, biobank);

            biobank.LastUpdated = DateTime.Now;

            // if it doesn't already have an anonymous id, make one
            if (!biobank.AnonymousIdentifier.HasValue)
                biobank.AnonymousIdentifier = Guid.NewGuid();

            await _organisationRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsBiobankSuspendedAsync(biobankDto.OrganisationId))
                await _indexService.UpdateBiobankDetails(biobank.OrganisationId);

            return biobank;
        }

        public async Task<OrganisationUser> AddUserToBiobankAsync(string userId, int biobankId)
        {
            //Validate the id's? user is more annoying as needs userManager

            var ou = new OrganisationUser
            {
                OrganisationId = biobankId,
                OrganisationUserId = userId
            };

            _organisationUserRepository.Insert(ou);
            var result = await _organisationUserRepository.SaveChangesAsync();
            if (result != 1) throw new DataException(); //should only be inserting the oud entity, definitely not adding a new org!

            return ou;
        }

        public async Task RemoveUserFromBiobankAsync(string userId, int biobankId)
        {
            await
                _organisationUserRepository.DeleteWhereAsync(
                    x => x.OrganisationUserId == userId && x.OrganisationId == biobankId);

            await _organisationUserRepository.SaveChangesAsync();
        }

        public async Task<OrganisationRegisterRequest> AddRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            _organisationRegisterRequestRepository.Insert(request);
            await _organisationRegisterRequestRepository.SaveChangesAsync();

            return request;
        }

        public async Task DeleteRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            await _organisationRegisterRequestRepository.DeleteAsync(request.OrganisationRegisterRequestId);
            await _organisationRegisterRequestRepository.SaveChangesAsync();
        }

        public async Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services)
        {
            foreach (var service in services)
            {
                //Validate service id first - don't want to go around inserting new unnamed services
                if (await _serviceOfferingRepository.GetByIdAsync(service.ServiceId) != null)
                {
                    //now make sure the biobank doesn't already have this service listed
                    if ((await _organisationServiceOfferingRepository.ListAsync(false,
                        x => x.OrganisationId == service.OrganisationId && x.ServiceId == service.ServiceId))
                        .FirstOrDefault() == null)
                    {
                        _organisationServiceOfferingRepository.Insert(service);
                    }
                }
                //atm we just silently fail if the service id is invalid; should we be throwing?
            }
            await _organisationServiceOfferingRepository.SaveChangesAsync();
        }

        public async Task DeleteBiobankServiceAsync(int biobankId, int serviceId)
        {
            //make sure the biobank has this service
            if ((await _organisationServiceOfferingRepository.ListAsync(false,
                x => x.OrganisationId == biobankId && x.ServiceId == serviceId))
                .FirstOrDefault() != null)
            {
                await
                _organisationServiceOfferingRepository.DeleteWhereAsync(
                    x => x.OrganisationId == biobankId && x.ServiceId == serviceId);
            }

            await _organisationServiceOfferingRepository.SaveChangesAsync();
        }

        public async Task<Network> CreateNetworkAsync(Network network)
        {
            _networkRepository.Insert(network);
            var result = await _networkRepository.SaveChangesAsync();
            if (result != 1) throw new DataException(); //shouldn't be doing anything other than inserting one network!

            return network;
        }

        public async Task<Network> UpdateNetworkAsync(NetworkDTO networkDto)
        {
            // get the network entity and update it, update elastic, return network
            var network = await _networkRepository.GetByIdAsync(networkDto.NetworkId);

            _mapper.Map(networkDto, network);

            network.LastUpdated = DateTime.Now;

            await _networkRepository.SaveChangesAsync();

            await _indexService.UpdateNetwork(network.NetworkId);

            return network;
        }

        public async Task<NetworkUser> AddUserToNetworkAsync(string userId, int networkId)
        {
            //Validate the id's? user is more annoying as needs userManager

            var nu = new NetworkUser
            {
                NetworkId = networkId,
                NetworkUserId = userId
            };

            _networkUserRepository.Insert(nu);
            var result = await _networkUserRepository.SaveChangesAsync();
            if (result != 1) throw new DataException(); //should only be inserting the nu entity, definitely not adding a new network!

            return nu;
        }

        public async Task RemoveUserFromNetworkAsync(string userId, int networkId)
        {
            await
                _networkUserRepository.DeleteWhereAsync(
                    x => x.NetworkUserId == userId && x.NetworkId == networkId);

            await _networkUserRepository.SaveChangesAsync();
        }

        public async Task<NetworkRegisterRequest> AddNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            _networkRegisterRequestRepository.Insert(request);
            await _networkRegisterRequestRepository.SaveChangesAsync();

            return request;
        }

        public async Task DeleteNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            await _networkRegisterRequestRepository.DeleteAsync(request.NetworkRegisterRequestId);
            await _networkRegisterRequestRepository.SaveChangesAsync();
        }

        public async Task<bool> AddBiobankToNetworkAsync(int biobankId, int networkId, string biobankExternalID)
        {
            var bb = await _organisationRepository.GetByIdAsync(biobankId);

            if (bb == null || bb.IsSuspended) throw new ApplicationException();

            var no = new OrganisationNetwork
            {
                NetworkId = networkId,
                OrganisationId = biobankId,
                ExternalID = biobankExternalID
            };

            try
            {
                _networkOrganisationRepository.Insert(no);
                await _networkOrganisationRepository.SaveChangesAsync();

                if (!await _biobankReadService.IsBiobankSuspendedAsync(biobankId))
                    await _indexService.JoinOrLeaveNetwork(biobankId);

                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId)
        {
            var bb = await _organisationRepository.GetByIdAsync(biobankId);

            if (bb == null || bb.IsSuspended) throw new ApplicationException();

            await _networkOrganisationRepository.DeleteWhereAsync(
                    x => x.NetworkId == networkId && x.OrganisationId == biobankId);

            await _networkOrganisationRepository.SaveChangesAsync();

            if (!await _biobankReadService.IsBiobankSuspendedAsync(biobankId))
                await _indexService.JoinOrLeaveNetwork(biobankId);
        }

        public async Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            var trackedRequest = await _organisationRegisterRequestRepository
                .GetByIdAsync(request.OrganisationRegisterRequestId);
            _mapper.Map(request, trackedRequest);

            await _organisationRegisterRequestRepository.SaveChangesAsync();

            return request;
        }

        public async Task<NetworkRegisterRequest> UpdateNetworkRegisterRequestAsync(NetworkRegisterRequest request)
        {
            var trackedRequest = await _networkRegisterRequestRepository
                .GetByIdAsync(request.NetworkRegisterRequestId);
            _mapper.Map(request, trackedRequest);

            await _networkRegisterRequestRepository.SaveChangesAsync();

            return request;
        }

        public async Task DeleteDiagnosisAsync(Diagnosis diagnosis)
        {
            await _diagnosisRepository.DeleteAsync(diagnosis.DiagnosisId);
            await _diagnosisRepository.SaveChangesAsync();
        }

        public async Task<Diagnosis> UpdateDiagnosisAsync(Diagnosis diagnosis)
        {
            _diagnosisRepository.Update(diagnosis);
            await _diagnosisRepository.SaveChangesAsync();

            return diagnosis;
        }

        public async Task<Diagnosis> AddDiagnosisAsync(Diagnosis diagnosis)
        {
            _diagnosisRepository.Insert(diagnosis);
            await _diagnosisRepository.SaveChangesAsync();

            return diagnosis;
        }

        #region RefData: Sample Collection Mode
        public async Task<SampleCollectionMode> AddSampleCollectionModeAsync(SampleCollectionMode sampleCollectionMode)
        {
            _sampleCollectionModeRepository.Insert(sampleCollectionMode);
            await _sampleCollectionModeRepository.SaveChangesAsync();

            return sampleCollectionMode;
        }

        public async Task<SampleCollectionMode> UpdateSampleCollectionModeAsync(SampleCollectionMode sampleCollectionMode, bool sortOnly = false)
        {
            var modes = await _biobankReadService.ListSampleCollectionModeAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                sampleCollectionMode.Description =
                    modes
                        .Where(x => x.SampleCollectionModeId == sampleCollectionMode.SampleCollectionModeId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldMode = modes.Where(x => x.SampleCollectionModeId == sampleCollectionMode.SampleCollectionModeId).First();
            var reverse = (oldMode.SortOrder < sampleCollectionMode.SortOrder);

            var newOrder = modes
                    .Prepend(sampleCollectionMode)
                    .GroupBy(x => x.SampleCollectionModeId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_sampleCollectionModeRepository.Update);

            await _sampleCollectionModeRepository.SaveChangesAsync();

            return sampleCollectionMode;
        }

        public async Task DeleteSampleCollectionModeAsync(SampleCollectionMode sampleCollectionMode)
        {
            await _sampleCollectionModeRepository.DeleteAsync(sampleCollectionMode.SampleCollectionModeId);
            await _sampleCollectionModeRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Collection Point
        public async Task<CollectionPoint> AddCollectionPointAsync(CollectionPoint collectionPoint)
        {
            _collectionPointRepository.Insert(collectionPoint);
            await _collectionPointRepository.SaveChangesAsync();

            return collectionPoint;
        }

        public async Task<CollectionPoint> UpdateCollectionPointAsync(CollectionPoint collectionPoint, bool sortOnly = false)
        {
            var points = await _biobankReadService.ListCollectionPointsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                collectionPoint.Description =
                    points
                        .Where(x => x.CollectionPointId == collectionPoint.CollectionPointId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldPoint = points.Where(x => x.CollectionPointId == collectionPoint.CollectionPointId).First();
            var reverse = (oldPoint.SortOrder < collectionPoint.SortOrder);

            var newOrder = points
                    .Prepend(collectionPoint)
                    .GroupBy(x => x.CollectionPointId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_collectionPointRepository.Update);

            await _collectionPointRepository.SaveChangesAsync();

            return collectionPoint;
        }

        public async Task DeleteCollectionPointAsync(CollectionPoint collectionPoint)
        {
            await _collectionPointRepository.DeleteAsync(collectionPoint.CollectionPointId);
            await _collectionPointRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Collection Percentage
        public async Task<CollectionPercentage> AddCollectionPercentageAsync(CollectionPercentage collectionPercentage)
        {
            _collectionPercentageRepository.Insert(collectionPercentage);
            await _collectionPercentageRepository.SaveChangesAsync();

            return collectionPercentage;
        }

        public async Task<CollectionPercentage> UpdateCollectionPercentageAsync(CollectionPercentage collectionPercentage, bool sortOnly = false)
        {
            var percentages = await _biobankReadService.ListCollectionPercentagesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                collectionPercentage.Description =
                    percentages
                        .Where(x => x.CollectionPercentageId == collectionPercentage.CollectionPercentageId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldPercentage = percentages.Where(x => x.CollectionPercentageId == collectionPercentage.CollectionPercentageId).First();
            var reverse = (oldPercentage.SortOrder < collectionPercentage.SortOrder);

            var newOrder = percentages
                    .Prepend(collectionPercentage)
                    .GroupBy(x => x.CollectionPercentageId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_collectionPercentageRepository.Update);

            await _collectionPercentageRepository.SaveChangesAsync();

            return collectionPercentage;
        }

        public async Task DeleteCollectionPercentageAsync(CollectionPercentage collectionPercentage)
        {
            await _collectionPercentageRepository.DeleteAsync(collectionPercentage.CollectionPercentageId);
            await _collectionPercentageRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Consent Restrictions
        public async Task<ConsentRestriction> AddConsentRestrictionAsync(ConsentRestriction consentRestriction)
        {
            _consentRestrictionRepository.Insert(consentRestriction);
            await _consentRestrictionRepository.SaveChangesAsync();

            return consentRestriction;
        }

        public async Task<ConsentRestriction> UpdateConsentRestrictionAsync(ConsentRestriction consentRestriction, bool sortOnly = false)
        {
            var restrictions = await _biobankReadService.ListConsentRestrictionsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                consentRestriction.Description =
                    restrictions
                        .Where(x => x.ConsentRestrictionId == consentRestriction.ConsentRestrictionId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldRestriction = restrictions.Where(x => x.ConsentRestrictionId == consentRestriction.ConsentRestrictionId).First();
            var reverse = (oldRestriction.SortOrder < consentRestriction.SortOrder);

            var newOrder = restrictions
                    .Prepend(consentRestriction)
                    .GroupBy(x => x.ConsentRestrictionId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_consentRestrictionRepository.Update);

            await _consentRestrictionRepository.SaveChangesAsync();

            return consentRestriction;
        }

        public async Task DeleteConsentRestrictionAsync(ConsentRestriction consentRestriction)
        {
            await _consentRestrictionRepository.DeleteAsync(consentRestriction.ConsentRestrictionId);
            await _consentRestrictionRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Donor Count
        public async Task<DonorCount> AddDonorCountAsync(DonorCount donorCount)
        {
            _donorCountRepository.Insert(donorCount);
            await _donorCountRepository.SaveChangesAsync();

            return donorCount;
        }

        public async Task<DonorCount> UpdateDonorCountAsync(DonorCount donorCount, bool sortOnly = false)
        {
            var donorCounts = await _biobankReadService.ListDonorCountsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                donorCount.Description =
                    donorCounts
                        .Where(x => x.DonorCountId == donorCount.DonorCountId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldCount = donorCounts.Where(x => x.DonorCountId == donorCount.DonorCountId).First();
            var reverse = (oldCount.SortOrder < donorCount.SortOrder);

            var newOrder = donorCounts
                    .Prepend(donorCount)
                    .GroupBy(x => x.DonorCountId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_donorCountRepository.Update);

            await _donorCountRepository.SaveChangesAsync();

            return donorCount;
        }

        public async Task DeleteDonorCountAsync(DonorCount donorCount)
        {
            await _donorCountRepository.DeleteAsync(donorCount.DonorCountId);
            await _donorCountRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Macroscopic Assessment
        public async Task<MacroscopicAssessment> AddMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment)
        {
            _macroscopicAssessmentRepository.Insert(macroscopicAssessment);
            await _macroscopicAssessmentRepository.SaveChangesAsync();

            return macroscopicAssessment;
        }
        
        public async Task<MacroscopicAssessment> UpdateMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment, bool sortOnly = false)
        {
            var assessments = await _biobankReadService.ListMacroscopicAssessmentsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                macroscopicAssessment.Description =
                    assessments
                        .Where(x => x.MacroscopicAssessmentId == macroscopicAssessment.MacroscopicAssessmentId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldAssessment = assessments.Where(x => x.MacroscopicAssessmentId == macroscopicAssessment.MacroscopicAssessmentId).First();
            var reverse = (oldAssessment.SortOrder < macroscopicAssessment.SortOrder);

            var newOrder = assessments
                    .Prepend(macroscopicAssessment)
                    .GroupBy(x => x.MacroscopicAssessmentId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_macroscopicAssessmentRepository.Update);

            await _macroscopicAssessmentRepository.SaveChangesAsync();

            return macroscopicAssessment;
        }

        public async Task DeleteMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment)
        {
            await _macroscopicAssessmentRepository.DeleteAsync(macroscopicAssessment.MacroscopicAssessmentId);
            await _macroscopicAssessmentRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Age Range
        public async Task<AgeRange> AddAgeRangeAsync(AgeRange ageRange)
        {
            _ageRangeRepository.Insert(ageRange);
            await _ageRangeRepository.SaveChangesAsync();

            return ageRange;
        }

        public async Task<AgeRange> UpdateAgeRangeAsync(AgeRange ageRange, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListAgeRangesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                ageRange.Description =
                    types
                        .Where(x => x.AgeRangeId == ageRange.AgeRangeId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.AgeRangeId == ageRange.AgeRangeId).First();
            var reverse = (oldType.SortOrder < ageRange.SortOrder);

            var newOrder = types
                    .Prepend(ageRange)
                    .GroupBy(x => x.AgeRangeId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_ageRangeRepository.Update);

            await _ageRangeRepository.SaveChangesAsync();

            return ageRange;
        }

        public async Task DeleteAgeRangeAsync(AgeRange ageRange)
        {
            await _ageRangeRepository.DeleteAsync(ageRange.AgeRangeId);
            await _ageRangeRepository.SaveChangesAsync();
        }
        #endregion
        
        #region RefData: Sop Status
        public async Task<SopStatus> AddSopStatusAsync(SopStatus sopStatus)
        {
            _sopStatusRepository.Insert(sopStatus);
            await _sopStatusRepository.SaveChangesAsync();

            return sopStatus;
        }

        public async Task<SopStatus> UpdateSopStatusAsync(SopStatus sopStatus, bool sortOnly = false)
        {
            var statuses = await _biobankReadService.ListSopStatusesAsync();
            
            // If only updating sortOrder
            if (sortOnly)
            {
                sopStatus.Description =
                    statuses
                        .Where(x => x.SopStatusId == sopStatus.SopStatusId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldStatus = statuses.Where(x => x.SopStatusId == sopStatus.SopStatusId).First();
            var reverse = (oldStatus.SortOrder < sopStatus.SortOrder);

            var newOrder = statuses
                    .Prepend(sopStatus)
                    .GroupBy(x => x.SopStatusId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_sopStatusRepository.Update);

            await _sopStatusRepository.SaveChangesAsync();

            return sopStatus;
        }

        public async Task DeleteSopStatusAsync(SopStatus sopStatus)
        {
            await _sopStatusRepository.DeleteAsync(sopStatus.SopStatusId);
            await _sopStatusRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Access Conditions
        public async Task<AccessCondition> AddAccessConditionAsync(AccessCondition accessCondition)
        {
            _accessConditionRepository.Insert(accessCondition);
            await _accessConditionRepository.SaveChangesAsync();

            return accessCondition;
        }

        public async Task<AccessCondition> UpdateAccessConditionAsync(AccessCondition accessCondition, bool sortOnly = false)
        {
            var conditions = await _biobankReadService.ListAccessConditionsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                accessCondition.Description =
                    conditions
                        .Where(x => x.AccessConditionId == accessCondition.AccessConditionId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldCondition = conditions.Where(x => x.AccessConditionId == accessCondition.AccessConditionId).First();
            var reverse = (oldCondition.SortOrder < accessCondition.SortOrder);

            var newOrder = conditions
                    .Prepend(accessCondition)
                    .GroupBy(x => x.AccessConditionId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_accessConditionRepository.Update);

            await _accessConditionRepository.SaveChangesAsync();

            return accessCondition;
        }

        public async Task DeleteAccessConditionAsync(AccessCondition accessCondition)
        {
            await _accessConditionRepository.DeleteAsync(accessCondition.AccessConditionId);
            await _accessConditionRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Annual Statistic
        public async Task<AnnualStatistic> AddAnnualStatisticAsync(AnnualStatistic annualStatistic)
        {
            _annualStatisticRepository.Insert(annualStatistic);
            await _annualStatisticRepository.SaveChangesAsync();

            return annualStatistic;
        }

        public async Task<AnnualStatistic> UpdateAnnualStatisticAsync(AnnualStatistic annualStatistic, bool sortOnly = false)
        {
            _annualStatisticRepository.Update(annualStatistic);
            await _annualStatisticRepository.SaveChangesAsync();

            return annualStatistic;
        }

        public async Task DeleteAnnualStatisticAsync(AnnualStatistic annualStatistic)
        {
            await _annualStatisticRepository.DeleteAsync(annualStatistic.AnnualStatisticId);
            await _annualStatisticRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Collection Status
        public async Task<CollectionStatus> AddCollectionStatusAsync(CollectionStatus collectionStatus)
        {
            _collectionStatusRepository.Insert(collectionStatus);
            await _collectionStatusRepository.SaveChangesAsync();

            return collectionStatus;
        }

        public async Task<CollectionStatus> UpdateCollectionStatusAsync(CollectionStatus collectionStatus, bool sortOnly = false)
        {
            var statuses = await _biobankReadService.ListCollectionStatusesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                collectionStatus.Description =
                    statuses
                        .Where(x => x.CollectionStatusId == collectionStatus.CollectionStatusId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldStatus = statuses.Where(x => x.CollectionStatusId == collectionStatus.CollectionStatusId).First();
            var reverse = (oldStatus.SortOrder < collectionStatus.SortOrder);

            var newOrder = statuses
                    .Prepend(collectionStatus)
                    .GroupBy(x => x.CollectionStatusId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_collectionStatusRepository.Update);

            await _collectionStatusRepository.SaveChangesAsync();

            return collectionStatus;
        }

        public async Task DeleteCollectionStatusAsync(CollectionStatus collectionStatus)
        {
            await _collectionStatusRepository.DeleteAsync(collectionStatus.CollectionStatusId);
            await _collectionStatusRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Associated Data Procurement Time Frame
        public async Task<AssociatedDataProcurementTimeframe> AddAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe timeframe)
        {
            _associatedDataProcurementTimeFrameRepository.Insert(timeframe);
            await _associatedDataProcurementTimeFrameRepository.SaveChangesAsync();

            return timeframe;
        }

        public async Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe timeframe, bool sortOnly = false)
        {
            var procurements = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();

            // If only updating sortOrder
            if (sortOnly)
            {
                timeframe.Description =
                    procurements
                        .Where(x => x.AssociatedDataProcurementTimeframeId == timeframe.AssociatedDataProcurementTimeframeId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldTimeframe = procurements.Where(x => x.AssociatedDataProcurementTimeframeId == timeframe.AssociatedDataProcurementTimeframeId).First();
            var reverse = (oldTimeframe.SortOrder < timeframe.SortOrder);

            var newOrder = procurements
                    .Prepend(timeframe)
                    .GroupBy(x => x.AssociatedDataProcurementTimeframeId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_associatedDataProcurementTimeFrameRepository.Update);

            await _associatedDataProcurementTimeFrameRepository.SaveChangesAsync();

            return timeframe;
        }

        public async Task DeleteAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
        {
            await _associatedDataProcurementTimeFrameRepository.DeleteAsync(associatedDataProcurementTimeframe.AssociatedDataProcurementTimeframeId);
            await _associatedDataProcurementTimeFrameRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: HTA Status
        public async Task<HtaStatus> AddHtaStatusAsync(HtaStatus htaStatus)
        {
            _htaStatusRepository.Insert(htaStatus);
            await _htaStatusRepository.SaveChangesAsync();

            return htaStatus;
        }

        public async Task<HtaStatus> UpdateHtaStatusAsync(HtaStatus htaStatus, bool sortOnly = false)
        {
            var statuses = await _biobankReadService.ListHtaStatusesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                htaStatus.Description =
                    statuses
                        .Where(x => x.HtaStatusId == htaStatus.HtaStatusId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldStatus = statuses.Where(x => x.HtaStatusId == htaStatus.HtaStatusId).First();
            var reverse = (oldStatus.SortOrder < htaStatus.SortOrder);

            var newOrder = statuses
                    .Prepend(htaStatus)
                    .GroupBy(x => x.HtaStatusId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_htaStatusRepository.Update);

            await _htaStatusRepository.SaveChangesAsync();

            return htaStatus;
        }

        public async Task DeleteHtaStatusAsync(HtaStatus htaStatus)
        {
            await _htaStatusRepository.DeleteAsync(htaStatus.HtaStatusId);
            await _htaStatusRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Preservation Types
        public async Task<PreservationType> AddPreservationTypeAsync(PreservationType preservationType)
        {
            _preservationTypeRepository.Insert(preservationType);
            await _preservationTypeRepository.SaveChangesAsync();

            return preservationType;
        }

        public async Task<PreservationType> UpdatePreservationTypeAsync(PreservationType preservationType, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListPreservationTypesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                preservationType.Description =
                    types
                        .Where(x => x.PreservationTypeId == preservationType.PreservationTypeId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.PreservationTypeId == preservationType.PreservationTypeId).First();
            var reverse = (oldType.SortOrder < preservationType.SortOrder);

            var newOrder = types
                    .Prepend(preservationType)
                    .GroupBy(x => x.PreservationTypeId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_preservationTypeRepository.Update);

            await _preservationTypeRepository.SaveChangesAsync();

            return preservationType;
        }

        public async Task DeletePreservationTypeAsync(PreservationType preservationType)
        {
            await _preservationTypeRepository.DeleteAsync(preservationType.PreservationTypeId);
            await _preservationTypeRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Material Type
        public async Task DeleteMaterialTypeAsync(MaterialType materialType)
        {
            await _materialTypeRepository.DeleteAsync(materialType.MaterialTypeId);
            await _materialTypeRepository.SaveChangesAsync();
        }

        public async Task<MaterialType> UpdateMaterialTypeAsync(MaterialType materialType, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListMaterialTypesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                materialType.Description =
                    types
                        .Where(x => x.MaterialTypeId == materialType.MaterialTypeId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.MaterialTypeId == materialType.MaterialTypeId).First();
            var reverse = (oldType.SortOrder < materialType.SortOrder);

            var newOrder = types
                    .Prepend(materialType)
                    .GroupBy(x => x.MaterialTypeId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_materialTypeRepository.Update);

            await _materialTypeRepository.SaveChangesAsync();

            return materialType;
        }

        public async Task<MaterialType> AddMaterialTypeAsync(MaterialType materialType)
        {
            _materialTypeRepository.Insert(materialType);
            await _materialTypeRepository.SaveChangesAsync();

            return materialType;
        }
        #endregion

        #region RefData: Collection Type
        public async Task DeleteCollectionTypeAsync(CollectionType collectionType)
        {
            await _collectionTypeRepository.DeleteAsync(collectionType.CollectionTypeId);
            await _collectionTypeRepository.SaveChangesAsync();
        }

        public async Task<CollectionType> UpdateCollectionTypeAsync(CollectionType collectionType, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListCollectionTypesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                collectionType.Description =
                    types
                        .Where(x => x.CollectionTypeId == collectionType.CollectionTypeId)
                        .First()
                        .Description;
            }

            // Insert respecting Sort Order
            types
                .Prepend(collectionType)          // Add new item
                .GroupBy(x => x.CollectionTypeId) // Remove old item of same ID
                .Select(x => x.First())
                .OrderByDescending(x => x.SortOrder)    // Order giving priority to new item
                .Reverse()
                .Select((x, i) =>                       // Reindex, starting at 1
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_collectionTypeRepository.Update);

            await _collectionTypeRepository.SaveChangesAsync();

            return collectionType;
        }

        public async Task<CollectionType> AddCollectionTypeAsync(CollectionType collectionType)
        {
            _collectionTypeRepository.Insert(collectionType);
            await _collectionTypeRepository.SaveChangesAsync();

            return collectionType;
        }
        #endregion

        #region RefData: Service Offerings
        public async Task<ServiceOffering> AddServiceOfferingAsync(ServiceOffering serviceOffering)
        {
            _serviceOfferingRepository.Insert(serviceOffering);
            await _serviceOfferingRepository.SaveChangesAsync();

            return serviceOffering;
        }

        public async Task<ServiceOffering> UpdateServiceOfferingAsync (ServiceOffering serviceOffering, bool sortOnly = false)
        {
            var offerings = await _biobankReadService.ListServiceOfferingsAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                serviceOffering.Name =
                    offerings
                        .Where(x => x.ServiceId == serviceOffering.ServiceId)
                        .First()
                        .Name;
            }

            // Add new item, remove old
            var oldStatus = offerings.Where(x => x.ServiceId == serviceOffering.ServiceId).First();
            var reverse = (oldStatus.SortOrder < serviceOffering.SortOrder);

            var newOrder = offerings
                    .Prepend(serviceOffering)
                    .GroupBy(x => x.ServiceId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_serviceOfferingRepository.Update);

            await _serviceOfferingRepository.SaveChangesAsync();

            return serviceOffering;
        }
      
        public async Task DeleteServiceOfferingAsync(ServiceOffering serviceOffering)
        {
            await _serviceOfferingRepository.DeleteAsync(serviceOffering.ServiceId);
            await _serviceOfferingRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Sex
        public async Task<Sex> AddSexAsync(Sex sex)
        {
            _sexRepository.Insert(sex);
            await _sexRepository.SaveChangesAsync();

            return sex;
        }

        public async Task<Sex> UpdateSexAsync(Sex sex, bool sortOnly = false)
        {
            var sexes = await _biobankReadService.ListSexesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                sex.Description =
                    sexes
                        .Where(x => x.SexId == sex.SexId)
                        .First()
                        .Description;
            }

            // Add new item, remove old
            var oldType = sexes.Where(x => x.SexId == sex.SexId).First();
            var reverse = (oldType.SortOrder < sex.SortOrder);

            var newOrder = sexes
                    .Prepend(sex)
                    .GroupBy(x => x.SexId)
                    .Select(x => x.First());

            // Sort depending on direction of change
            newOrder = reverse
                    ? newOrder.OrderByDescending(x => x.SortOrder).Reverse()
                    : newOrder.OrderBy(x => x.SortOrder);

            // Re-index and update
            newOrder
                .Select((x, i) =>
                {
                    x.SortOrder = (i + 1);
                    return x;
                })
                .ToList()
                .ForEach(_sexRepository.Update);

            await _sexRepository.SaveChangesAsync();

            return sex;
        }

        public async Task DeleteSexAsync(Sex sex)
        {
            await _sexRepository.DeleteAsync(sex.SexId);
            await _sexRepository.SaveChangesAsync();
        }
        #endregion

        public async Task UpdateSiteConfigsAsync(IEnumerable<Config> configs)
        {
            foreach (var config in configs) {
                var oldConfig = await _biobankReadService.GetSiteConfig(config.Key);
                oldConfig.Value = config.Value;

                _siteConfigRepository.Update(oldConfig);
            }

            await _siteConfigRepository.SaveChangesAsync();
        }

        //delete adt
        public async Task DeleteAssociatedDataTypeAsync(AssociatedDataType associatedDataType)
        {
            await _associatedDataTypeRepository.DeleteAsync(associatedDataType.AssociatedDataTypeId);
            await _associatedDataTypeRepository.SaveChangesAsync();
        }
        public async Task<AssociatedDataType> UpdateAssociatedDataTypeAsync(AssociatedDataType associatedDataType)
        {
            _associatedDataTypeRepository.Update(associatedDataType);
            await _associatedDataTypeRepository.SaveChangesAsync();

            return associatedDataType;
        }

        public async Task<AssociatedDataType> AddAssociatedDataTypeAsync(AssociatedDataType associatedDataType)
        {
            _associatedDataTypeRepository.Insert(associatedDataType);
            await _associatedDataTypeRepository.SaveChangesAsync();

            return associatedDataType;
        }

        #region RefData: Country
        public async Task<Country> AddCountryAsync(Country country)
        {
            _countryRepository.Insert(country);
            await _countryRepository.SaveChangesAsync();

            return country;
        }

        public async Task<Country> UpdateCountryAsync(Country country)
        {
            _countryRepository.Update(country);
            await _countryRepository.SaveChangesAsync();

            return country;
        }

        public async Task DeleteCountryAsync(Country country)
        {
            await _countryRepository.DeleteAsync(country.CountryId);
            await _countryRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: County
        public async Task<County> AddCountyAsync(County county) {
            _countyRepository.Insert(county);
            await _countyRepository.SaveChangesAsync();

            return county;
        }

        public async Task<County> UpdateCountyAsync(County county) {
            _countyRepository.Update(county);
            await _countyRepository.SaveChangesAsync();

            return county;
        }
        public async Task DeleteCountyAsync(County county)
        {
            await _countyRepository.DeleteAsync(county.CountyId);
            await _countyRepository.SaveChangesAsync();
        }

        #endregion
        
        public async Task DeleteRegistrationReasonAsync(RegistrationReason registrationReason)
        {
            await _registrationReasonRepository.DeleteAsync(registrationReason.RegistrationReasonId);
            await _registrationReasonRepository.SaveChangesAsync();
        }
        public async Task<RegistrationReason> UpdateRegistrationReasonAsync(RegistrationReason registrationReason)
        {
            _registrationReasonRepository.Update(registrationReason);
            await _registrationReasonRepository.SaveChangesAsync();
            return registrationReason;
        }

        public async Task<RegistrationReason> AddRegistrationReasonAsync(RegistrationReason registrationReason)
        {
            _registrationReasonRepository.Insert(registrationReason);
            await _registrationReasonRepository.SaveChangesAsync();

            return registrationReason;
        }
        public async Task<Organisation> SuspendBiobankAsync(int id)
        {
            var biobank = await _organisationRepository.GetByIdAsync(id);

            if (biobank == null) throw new KeyNotFoundException();

            //Mark it as suspended
            biobank.IsSuspended = true;

            //Update
            _organisationRepository.Update(biobank);
            await _organisationRepository.SaveChangesAsync();

            //Arrange removal from the search index
            await _indexService.BulkDeleteBiobank(id);



            return biobank;
        }

        public async Task<Organisation> UnsuspendBiobankAsync(int id)
        {
            var biobank = await _organisationRepository.GetByIdAsync(id);

            if (biobank == null) throw new KeyNotFoundException();

            //Mark it as not suspended
            biobank.IsSuspended = false;

            //Update
            _organisationRepository.Update(biobank);
            await _organisationRepository.SaveChangesAsync();

            //Re-index Biobank data (entity needs to be updated before we do this)
            await _indexService.BulkIndexBiobank(id);

            return biobank;
        }

        public async Task<bool> AddFunderToBiobankAsync(int funderId, int biobankId)
        {
            var funder = await _funderRepository.GetByIdAsync(funderId);
            var bb = await _organisationRepository.GetByIdAsync(biobankId);

            if (bb == null || funder == null) throw new ApplicationException();

            try
            {
                funder.Organisations.Add(bb);

                _funderRepository.Update(funder);
                await _funderRepository.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task RemoveFunderFromBiobankAsync(int funderId, int biobankId)
        {
            var funder = await _funderRepository.GetByIdAsync(funderId);

            if (funder == null) throw new ApplicationException();

            funder.Organisations.Remove(
                funder.Organisations
                    .FirstOrDefault(x => x.OrganisationId == biobankId));

            _funderRepository.Update(funder);
            await _funderRepository.SaveChangesAsync();
        }

        public async Task DeleteBiobankAsync(int id)
        {
            // remove biobank
            await _organisationRepository.DeleteAsync(id);
            await _organisationRepository.SaveChangesAsync();
        }

        public async Task DeleteFunderByIdAsync(int id)
        {
            await _funderRepository.DeleteAsync(id);
            await _funderRepository.SaveChangesAsync();
        }

        public async Task<Funder> AddFunderAsync(Funder funder)
        {
            _funderRepository.Insert(funder);
            await _funderRepository.SaveChangesAsync();

            return funder;
        }

        public async Task<Funder> UpdateFunderAsync(Funder funder)
        {
            _funderRepository.Update(funder);
            await _funderRepository.SaveChangesAsync();

            return funder;
        }

        public async Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year)
        {
            var organisationAnnualStatistic = new OrganisationAnnualStatistic
            {
                OrganisationId = organisationId,
                AnnualStatisticId = statisticId,
                Value = value,
                Year = year
            };

            var existingEntry = (await _organisationAnnualStatisticRepository.ListAsync(true,
                oas => oas.OrganisationId == organisationId
                       && oas.AnnualStatisticId == statisticId
                       && oas.Year == year)).FirstOrDefault();

            if (existingEntry != null)
            {
                existingEntry.Value = value;
                _organisationAnnualStatisticRepository.Update(existingEntry);
            }

            else
                _organisationAnnualStatisticRepository.Insert(organisationAnnualStatistic);

            await _organisationAnnualStatisticRepository.SaveChangesAsync();
        }

        public async Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons)
        {
            foreach (var registrationReason in activeRegistrationReasons)
            {
                //Validate reason id first - don't want to go around inserting new unnamed reasons
                if (await _registrationReasonRepository.GetByIdAsync(registrationReason.RegistrationReasonId) != null)
                {
                    //now make sure the biobank doesn't already have this reason listed
                    if ((await _organisationRegistrationReasonRepository.ListAsync(false,
                        x => x.OrganisationId == registrationReason.OrganisationId && x.RegistrationReasonId == registrationReason.RegistrationReasonId))
                        .FirstOrDefault() == null)
                    {
                        _organisationRegistrationReasonRepository.Insert(registrationReason);
                    }
                }
            }
            await _organisationRegistrationReasonRepository.SaveChangesAsync();
        }

        public async Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId)
        {
            //make sure the biobank has this reason
            if ((await _organisationRegistrationReasonRepository.ListAsync(false,
                x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId))
                .FirstOrDefault() != null)
            {
                await
                _organisationRegistrationReasonRepository.DeleteWhereAsync(
                    x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId);
            }

            await _organisationRegistrationReasonRepository.SaveChangesAsync();
        }
    
        public async Task<Publication> AddOrganisationPublicationAsync(Publication publication)
        {
            _publicationRespository.Insert(publication);
            await _publicationRespository.SaveChangesAsync();


            return publication;
        }

        public async Task DeleteAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup)
        {
            await _associatedDataTypeGroupRepository.DeleteAsync(associatedDataTypeGroup.AssociatedDataTypeGroupId);
            await _associatedDataTypeGroupRepository.SaveChangesAsync();
        }

        public async Task<AssociatedDataTypeGroup> AddAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup)
        {
            _associatedDataTypeGroupRepository.Insert(associatedDataTypeGroup);
            await _associatedDataTypeGroupRepository.SaveChangesAsync();

            return associatedDataTypeGroup;
        }
        public async Task<AssociatedDataTypeGroup> UpdateAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup)
        {
            _associatedDataTypeGroupRepository.Update(associatedDataTypeGroup);
            await _associatedDataTypeGroupRepository.SaveChangesAsync();

            return associatedDataTypeGroup;
        }
        public async Task DeleteAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup)
        {
            await _annualStatisticGroupRepository.DeleteAsync(annualStatisticGroup.AnnualStatisticGroupId);
            await _annualStatisticGroupRepository.SaveChangesAsync();
        }

        public async Task<AnnualStatisticGroup> UpdateAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup)
        {
            _annualStatisticGroupRepository.Update(annualStatisticGroup);
            await _annualStatisticGroupRepository.SaveChangesAsync();

            return annualStatisticGroup;
        }

        public async Task<AnnualStatisticGroup> AddAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup)
        {
            _annualStatisticGroupRepository.Insert(annualStatisticGroup);
            await _annualStatisticGroupRepository.SaveChangesAsync();

            return annualStatisticGroup;
        }

        #region Content Management
        public async Task<string> StoreBlobAsync(MemoryStream blob, string fileName, string contentType, string reference)
            => await _blobStorageProvider.StoreBlobAsync(blob, fileName, contentType, reference);

        public async Task<string> StoreImageAsync(Stream image, string fileName, string contentType, string reference, int maxX = 1200, int maxY = 1200)
            => await StoreBlobAsync(ImageService.ResizeImageStream(image, maxX, maxY), fileName, contentType, reference);

        public async Task DeleteBlobAsync(string resourceName)
            => await _blobStorageProvider.DeleteBlobAsync(resourceName);

        // TODO: Replace With Above Methods 
        public async Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference)
        {
            var resizedLogoStream = ImageService.ResizeImageStream(logoFileStream, maxX: 300, maxY: 300);

            return await _blobStorageProvider.StoreBlobAsync(resizedLogoStream, logoFileName, logoContentType, reference);
        }

        public async Task DeleteLogoAsync(int organisationId)
        {
            var organisation = await _biobankReadService.GetBiobankByIdAsync(organisationId);

            if (organisation != null)
            {
                await DeleteBlobAsync(organisation.Logo);
            }
        }
        #endregion
    }
}
