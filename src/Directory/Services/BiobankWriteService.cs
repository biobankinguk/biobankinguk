using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Directory.Data.Repositories;
using Biobanks.Directory.Data.Transforms.Url;
using System.IO;
using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Biobanks.Entities.Shared;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Extensions;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Services
{
    public class BiobankWriteService : IBiobankWriteService
    {
        #region Properties and ctor

        private readonly ICollectionService _collectionService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IConfigService _configService;

        private readonly ILogoStorageProvider _logoStorageProvider;

        private readonly IGenericEFRepository<OntologyTerm> _ontologyTermRepository;
        private readonly IGenericEFRepository<AgeRange> _ageRangeRepository;
        private readonly IGenericEFRepository<CollectionPercentage> _collectionPercentageRepository;
        private readonly IGenericEFRepository<DonorCount> _donorCountRepository;
        private readonly IGenericEFRepository<SampleCollectionMode> _sampleCollectionModeRepository;
        private readonly IGenericEFRepository<MacroscopicAssessment> _macroscopicAssessmentRepository;
        private readonly IGenericEFRepository<StorageTemperature> _storageTemperatureRepository;
        private readonly IGenericEFRepository<MaterialType> _materialTypeRepository;
        private readonly IGenericEFRepository<MaterialTypeGroup> _materialTypeGroupRepository;
        private readonly IGenericEFRepository<Sex> _sexRepository;
        private readonly IGenericEFRepository<SopStatus> _sopStatusRepository;
        private readonly IGenericEFRepository<AnnualStatistic> _annualStatisticRepository;
        private readonly IGenericEFRepository<AnnualStatisticGroup> _annualStatisticGroupRepository;
        private readonly IGenericEFRepository<AssociatedDataType> _associatedDataTypeRepository;
        private readonly IGenericEFRepository<AssociatedDataTypeGroup> _associatedDataTypeGroupRepository;
        private readonly IGenericEFRepository<AccessCondition> _accessConditionRepository;
        private readonly IGenericEFRepository<ConsentRestriction> _consentRestrictionRepository;
        private readonly IGenericEFRepository<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameRepository;
        private readonly IGenericEFRepository<CollectionType> _collectionTypeRepository;
        private readonly IGenericEFRepository<County> _countyRepository;
        private readonly IGenericEFRepository<CollectionStatus> _collectionStatusRepository;

        private readonly IGenericEFRepository<Collection> _collectionRepository;
        private readonly IGenericEFRepository<DiagnosisCapability> _capabilityRepository;
        private readonly IGenericEFRepository<SampleSet> _sampleSetRepository;
        private readonly IGenericEFRepository<MaterialDetail> _materialDetailRepository;

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
        private readonly IGenericEFRepository<OrganisationNetwork> _organisationNetworkRepository;
        private readonly IGenericEFRepository<RegistrationReason> _registrationReasonRepository;
        private readonly IGenericEFRepository<ServiceOffering> _serviceOfferingRepository;
        private readonly IGenericEFRepository<PreservationType> _preservationTypeRepository;

        private readonly IGenericEFRepository<Funder> _funderRepository;

        private readonly IGenericEFRepository<Config> _siteConfigRepository;

        private readonly IBiobankIndexService _indexService;

        private readonly IMapper _mapper;

        public BiobankWriteService(
            ICollectionService collectionService,
            IBiobankReadService biobankReadService,
            IConfigService configService,
            ILogoStorageProvider logoStorageProvider,
            IGenericEFRepository<OntologyTerm> ontologyTermRepository,
            IGenericEFRepository<MaterialType> materialTypeRepository,
            IGenericEFRepository<MaterialTypeGroup> materialTypeGroupRepository,
            IGenericEFRepository<Sex> sexRepository,
            IGenericEFRepository<AnnualStatistic> annualStatisticRepository,
            IGenericEFRepository<AnnualStatisticGroup> annualStatisticGroupRepository,
            IGenericEFRepository<AssociatedDataType> associatedDataTypeRepository,
            IGenericEFRepository<AssociatedDataTypeGroup> associatedDataTypeGroupRepository,
            IGenericEFRepository<CollectionPercentage> collectionPercentageRepository,
            IGenericEFRepository<DonorCount> donorCountRepository,
            IGenericEFRepository<CollectionType> collectionTypeRepository,
            IGenericEFRepository<CollectionStatus> collectionStatusRepository,
            IGenericEFRepository<AgeRange> ageRangeRepository,
            IGenericEFRepository<MacroscopicAssessment> macroscopicAssessmentRepository,
            IGenericEFRepository<SampleCollectionMode> sampleCollectionModeRepository,
            IGenericEFRepository<StorageTemperature> storageTemperatureRepository,
            IGenericEFRepository<AccessCondition> accessConditionRepository,
            IGenericEFRepository<SopStatus> sopStatusRepository,
            IGenericEFRepository<ConsentRestriction> consentRestrictionRepository,
            IGenericEFRepository<County> countyRepository,
            IGenericEFRepository<Collection> collectionRepository,
            IGenericEFRepository<DiagnosisCapability> capabilityRepository,
            IGenericEFRepository<SampleSet> sampleSetRepository,
            IGenericEFRepository<MaterialDetail> materialDetailRepository,
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
            IGenericEFRepository<OrganisationNetwork> organisationNetworkRepository,
            IGenericEFRepository<OrganisationType> organisationTypeRepository,
            IGenericEFRepository<OrganisationServiceOffering> organisationServiceOfferingRepository,
            IGenericEFRepository<RegistrationReason> registrationReasonRepository,
            IGenericEFRepository<ServiceOffering> serviceOfferingRepository,
            IGenericEFRepository<PreservationType> preservationTypeRepository,

            IGenericEFRepository<Config> siteConfigRepository,

            IBiobankIndexService indexService,
            IMapper mapper,

            IGenericEFRepository<Funder> funderRepository)
        {
            _collectionService = collectionService;

            _biobankReadService = biobankReadService;
            _configService = configService;
            _logoStorageProvider = logoStorageProvider;

            _ontologyTermRepository = ontologyTermRepository;
            _collectionPercentageRepository = collectionPercentageRepository;
            _donorCountRepository = donorCountRepository;
            _ageRangeRepository = ageRangeRepository;
            _macroscopicAssessmentRepository = macroscopicAssessmentRepository;
            _storageTemperatureRepository = storageTemperatureRepository;
            _sopStatusRepository = sopStatusRepository;
            _sampleCollectionModeRepository = sampleCollectionModeRepository;
            _associatedDataTypeRepository = associatedDataTypeRepository;
            _associatedDataTypeGroupRepository = associatedDataTypeGroupRepository;
            _annualStatisticRepository = annualStatisticRepository;
            _annualStatisticGroupRepository = annualStatisticGroupRepository;
            _accessConditionRepository = accessConditionRepository;
            _materialTypeRepository = materialTypeRepository;
            _materialTypeGroupRepository = materialTypeGroupRepository;
            _sexRepository = sexRepository;
            _consentRestrictionRepository = consentRestrictionRepository;
            _countyRepository = countyRepository;
            _collectionStatusRepository = collectionStatusRepository;
            _collectionTypeRepository = collectionTypeRepository;

            _collectionRepository = collectionRepository;
            _capabilityRepository = capabilityRepository;
            _sampleSetRepository = sampleSetRepository;
            _materialDetailRepository = materialDetailRepository;

            _networkRepository = networkRepository;
            _networkUserRepository = networkUserRepository;
            _networkRegisterRequestRepository = networkRegisterRequestRepository;
            _networkOrganisationRepository = networkOrganisationRepository;

            _organisationRepository = organisationRepository;
            _organisationAnnualStatisticRepository = organisationAnnualStatisticRepository;
            _organisationRegisterRequestRepository = organisationRegisterRequestRepository;
            _organisationRegistrationReasonRepository = organisationRegistrationReasonRepository;
            _organisationUserRepository = organisationUserRepository;
            _organisationNetworkRepository = organisationNetworkRepository;
            _organisationTypeRepository = organisationTypeRepository;
            _organisationServiceOfferingRepository = organisationServiceOfferingRepository;
            _registrationReasonRepository = registrationReasonRepository;
            _serviceOfferingRepository = serviceOfferingRepository;
            _associatedDataProcurementTimeFrameRepository = associatedDataProcurementTimeFrameRepository;
            _preservationTypeRepository = preservationTypeRepository;

            _siteConfigRepository = siteConfigRepository;

            _indexService = indexService;

            _mapper = mapper;
            _funderRepository = funderRepository;
        }

        #endregion

        public async Task AddSampleSetAsync(SampleSet sampleSet)
        {
            // Add new SampleSet
            _sampleSetRepository.Insert(sampleSet);
            await _sampleSetRepository.SaveChangesAsync();

            // Update collection's timestamp
            var collection = await _collectionRepository.GetByIdAsync(sampleSet.CollectionId);
            collection.LastUpdated = DateTime.Now;
            await _collectionRepository.SaveChangesAsync();

            // Index New SampleSet
            if (!await _biobankReadService.IsCollectionBiobankSuspendedAsync(sampleSet.CollectionId))
                await _indexService.IndexSampleSet(sampleSet.Id);
        }

        public async Task UpdateSampleSetAsync(SampleSet sampleSet)
        {
            // Update SampleSet
            var existingSampleSet = (await _sampleSetRepository.ListAsync(
                    tracking: true,
                    filter: x => x.Id == sampleSet.Id,
                    orderBy: null,
                    x => x.Collection,
                    x => x.MaterialDetails)
                )
                .First();

            existingSampleSet.SexId = sampleSet.SexId;
            existingSampleSet.AgeRangeId = sampleSet.AgeRangeId;
            existingSampleSet.DonorCountId = sampleSet.DonorCountId;
            existingSampleSet.Collection.LastUpdated = DateTime.Now;

            // Existing MaterialDetails
            foreach (var existingMaterialDetail in existingSampleSet.MaterialDetails.ToList())
            {
                var materialDetail = sampleSet.MaterialDetails.FirstOrDefault(x => x.Id == existingMaterialDetail.Id);

                // Update MaterialDetail 
                if (materialDetail != null)
                {
                    existingMaterialDetail.MaterialTypeId = materialDetail.MaterialTypeId;
                    existingMaterialDetail.StorageTemperatureId = materialDetail.StorageTemperatureId;
                    existingMaterialDetail.MacroscopicAssessmentId = materialDetail.MacroscopicAssessmentId;
                    existingMaterialDetail.ExtractionProcedureId = materialDetail.ExtractionProcedureId;
                    existingMaterialDetail.PreservationTypeId = materialDetail.PreservationTypeId;
                    existingMaterialDetail.CollectionPercentageId = materialDetail.CollectionPercentageId;
                }
                // Delete MaterialDetail
                else
                {
                    _materialDetailRepository.Delete(existingMaterialDetail);
                }
            }

            // New MaterialDetails
            foreach (var materialDetail in sampleSet.MaterialDetails.Where(x => x.Id == default))
            {
                _materialDetailRepository.Insert(
                    new MaterialDetail
                    {
                        SampleSetId = existingSampleSet.Id,
                        MaterialTypeId = materialDetail.MaterialTypeId,
                        StorageTemperatureId = materialDetail.StorageTemperatureId,
                        MacroscopicAssessmentId = materialDetail.MacroscopicAssessmentId,
                        ExtractionProcedureId = materialDetail.ExtractionProcedureId,
                        PreservationTypeId = materialDetail.PreservationTypeId,
                        CollectionPercentageId = materialDetail.CollectionPercentageId
                    }
                );
            }

            await _sampleSetRepository.SaveChangesAsync();
            await _materialDetailRepository.SaveChangesAsync();

            // Update Search Index
            if (!await _biobankReadService.IsCollectionBiobankSuspendedAsync(existingSampleSet.CollectionId))
            {
                await _indexService.UpdateSampleSetDetails(sampleSet.Id);
            }
        }

        public async Task DeleteSampleSetAsync(int id)
        {
            //we need to check if the sampleset belongs to a suspended bb, BEFORE we delete the sampleset
            var suspended = await _biobankReadService.IsSampleSetBiobankSuspendedAsync(id);

            //delete materialdetails to avoid orphaned data or integrity errors
            await _materialDetailRepository.DeleteWhereAsync(x => x.SampleSetId == id);
            await _materialDetailRepository.SaveChangesAsync();

            await _sampleSetRepository.DeleteWhereAsync(x => x.Id == id);

            await _sampleSetRepository.SaveChangesAsync();

            if (!suspended)
                _indexService.DeleteSampleSet(id);
        }

        public async Task AddCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
            var ontologyTerm = await _biobankReadService.GetOntologyTerm(description: capabilityDTO.OntologyTerm, onlyDisplayable: true);

            var capability = new DiagnosisCapability
            {
                OrganisationId = capabilityDTO.OrganisationId,
                OntologyTermId = ontologyTerm.Id,
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

            var ontologyTerm = await _biobankReadService.GetOntologyTerm(description: capabilityDTO.OntologyTerm, onlyDisplayable: true);

            existingCapability.OntologyTermId = ontologyTerm.Id;
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

        public async Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference)
        {
            var resizedLogoStream = ImageService.ResizeImageStream(logoFileStream, maxX: 300, maxY: 300);

            return await _logoStorageProvider.StoreLogoAsync(resizedLogoStream, logoFileName, logoContentType, reference);
        }

        public async Task RemoveLogoAsync(int organisationId)
        {
            await _logoStorageProvider.RemoveLogoAsync(organisationId);
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
                if (await _serviceOfferingRepository.GetByIdAsync(service.ServiceOfferingId) != null)
                {
                    //now make sure the biobank doesn't already have this service listed
                    if ((await _organisationServiceOfferingRepository.ListAsync(false,
                        x => x.OrganisationId == service.OrganisationId && x.ServiceOfferingId == service.ServiceOfferingId))
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
                x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId))
                .FirstOrDefault() != null)
            {
                await
                _organisationServiceOfferingRepository.DeleteWhereAsync(
                    x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId);
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

        public async Task<bool> AddBiobankToNetworkAsync(int biobankId, int networkId, string biobankExternalID, bool approve)
        {
            var bb = await _organisationRepository.GetByIdAsync(biobankId);

            if (bb == null || bb.IsSuspended) throw new ApplicationException();

            var no = new OrganisationNetwork
            {
                NetworkId = networkId,
                OrganisationId = biobankId,
                ExternalID = biobankExternalID
            };
            if (approve)
            {
                no.ApprovedDate = DateTime.Now;
            }

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

        public async Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork)
        {
            _organisationNetworkRepository.Update(organisationNetwork);
            await _organisationNetworkRepository.SaveChangesAsync();
            return organisationNetwork;
        }

        public async Task DeleteOntologyTermAsync(OntologyTerm ontologyTerm)
        {
            await _ontologyTermRepository.DeleteAsync(ontologyTerm.Id);
            await _ontologyTermRepository.SaveChangesAsync();
        }

        public async Task<OntologyTerm> UpdateOntologyTermAsync(OntologyTerm ontologyTerm)
        {
            _ontologyTermRepository.Update(ontologyTerm);
            await _ontologyTermRepository.SaveChangesAsync();

            // Update Indexed Capabilities
            await _indexService.UpdateCapabilitiesOntologyOtherTerms(ontologyTerm.Value);

            // Update Indexed Collections
            var collections = await _collectionService.ListByOntologyTerm(ontologyTerm.Value);

            foreach (var collection in collections)
            {
                _indexService.UpdateCollectionDetails(
                    await _collectionService.GetForIndexing(collection.CollectionId));
            }

            return ontologyTerm;
        }

        public async Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm ontologyTerm)
        {
            _ontologyTermRepository.Insert(ontologyTerm);
            await _ontologyTermRepository.SaveChangesAsync();

            return ontologyTerm;
        }

        public async Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds)
        {
            foreach (var mId in materialTypeIds)
            {
                (await _materialTypeRepository.ListAsync(true, x => x.Id == mId, null, x => x.ExtractionProcedures))
                    .FirstOrDefault().ExtractionProcedures
                    .Add(ontologyTerm);
                await _ontologyTermRepository.SaveChangesAsync();
            }
        }

        public async Task UpdateOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds)
        {
            await UpdateOntologyTermAsync(ontologyTerm);

            var Term = (await _ontologyTermRepository.ListAsync(true,x=>x.Id == ontologyTerm.Id,null, x=>x.MaterialTypes)).FirstOrDefault();
            var materialTypes = (await _materialTypeRepository.ListAsync(true, x => materialTypeIds.Contains(x.Id))).ToList();
            Term.MaterialTypes = materialTypes;

            await _ontologyTermRepository.SaveChangesAsync();
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
                sampleCollectionMode.Value =
                    modes
                        .Where(x => x.Id == sampleCollectionMode.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldMode = modes.Where(x => x.Id == sampleCollectionMode.Id).First();
            var reverse = (oldMode.SortOrder < sampleCollectionMode.SortOrder);

            var newOrder = modes
                    .Prepend(sampleCollectionMode)
                    .GroupBy(x => x.Id)
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
            await _sampleCollectionModeRepository.DeleteAsync(sampleCollectionMode.Id);
            await _sampleCollectionModeRepository.SaveChangesAsync();
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
                collectionPercentage.Value =
                    percentages
                        .Where(x => x.Id == collectionPercentage.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldPercentage = percentages.Where(x => x.Id == collectionPercentage.Id).First();
            var reverse = (oldPercentage.SortOrder < collectionPercentage.SortOrder);

            var newOrder = percentages
                    .Prepend(collectionPercentage)
                    .GroupBy(x => x.Id)
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
            await _collectionPercentageRepository.DeleteAsync(collectionPercentage.Id);
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
                consentRestriction.Value =
                    restrictions
                        .Where(x => x.Id == consentRestriction.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldRestriction = restrictions.Where(x => x.Id == consentRestriction.Id).First();
            var reverse = (oldRestriction.SortOrder < consentRestriction.SortOrder);

            var newOrder = restrictions
                    .Prepend(consentRestriction)
                    .GroupBy(x => x.Id)
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
            await _consentRestrictionRepository.DeleteAsync(consentRestriction.Id);
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
            var donorCounts = await _biobankReadService.ListDonorCountsAsync(true);

            // If only updating sortOrder
            if (sortOnly)
            {
                donorCount.Value =
                    donorCounts
                        .Where(x => x.Id == donorCount.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldCount = donorCounts.Where(x => x.Id == donorCount.Id).First();
            var reverse = (oldCount.SortOrder < donorCount.SortOrder);

            var newOrder = donorCounts
                    .Prepend(donorCount)
                    .GroupBy(x => x.Id)
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
            await _donorCountRepository.DeleteAsync(donorCount.Id);
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
                macroscopicAssessment.Value =
                    assessments
                        .Where(x => x.Id == macroscopicAssessment.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldAssessment = assessments.Where(x => x.Id == macroscopicAssessment.Id).First();
            var reverse = (oldAssessment.SortOrder < macroscopicAssessment.SortOrder);

            var newOrder = assessments
                    .Prepend(macroscopicAssessment)
                    .GroupBy(x => x.Id)
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
            await _macroscopicAssessmentRepository.DeleteAsync(macroscopicAssessment.Id);
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
                ageRange.Value =
                    types
                        .Where(x => x.Id == ageRange.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.Id == ageRange.Id).First();
            var reverse = (oldType.SortOrder < ageRange.SortOrder);

            var newOrder = types
                    .Prepend(ageRange)
                    .GroupBy(x => x.Id)
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
            await _ageRangeRepository.DeleteAsync(ageRange.Id);
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
                sopStatus.Value =
                    statuses
                        .Where(x => x.Id == sopStatus.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldStatus = statuses.Where(x => x.Id == sopStatus.Id).First();
            var reverse = (oldStatus.SortOrder < sopStatus.SortOrder);

            var newOrder = statuses
                    .Prepend(sopStatus)
                    .GroupBy(x => x.Id)
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
            await _sopStatusRepository.DeleteAsync(sopStatus.Id);
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
                accessCondition.Value =
                    conditions
                        .Where(x => x.Id == accessCondition.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldCondition = conditions.Where(x => x.Id == accessCondition.Id).First();
            var reverse = (oldCondition.SortOrder < accessCondition.SortOrder);

            var newOrder = conditions
                    .Prepend(accessCondition)
                    .GroupBy(x => x.Id)
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
            await _accessConditionRepository.DeleteAsync(accessCondition.Id);
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
            await _annualStatisticRepository.DeleteAsync(annualStatistic.Id);
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
                collectionStatus.Value =
                    statuses
                        .Where(x => x.Id == collectionStatus.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldStatus = statuses.Where(x => x.Id == collectionStatus.Id).First();
            var reverse = (oldStatus.SortOrder < collectionStatus.SortOrder);

            var newOrder = statuses
                    .Prepend(collectionStatus)
                    .GroupBy(x => x.Id)
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
            await _collectionStatusRepository.DeleteAsync(collectionStatus.Id);
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
                timeframe.Value =
                    procurements
                        .Where(x => x.Id == timeframe.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldTimeframe = procurements.Where(x => x.Id == timeframe.Id).First();
            var reverse = (oldTimeframe.SortOrder < timeframe.SortOrder);

            var newOrder = procurements
                    .Prepend(timeframe)
                    .GroupBy(x => x.Id)
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
            await _associatedDataProcurementTimeFrameRepository.DeleteAsync(associatedDataProcurementTimeframe.Id);
            await _associatedDataProcurementTimeFrameRepository.SaveChangesAsync();
        }
        #endregion

   
        #region RefData: PreservationType
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
                preservationType.Value = types.First(x => x.Id == preservationType.Id).Value;
            }

            // Add new item, remove old
            var oldType = types.First(x => x.Id == preservationType.Id);
            var reverse = (oldType.SortOrder < preservationType.SortOrder);

            var newOrder = types
                .Prepend(preservationType)
                .GroupBy(x => x.Id)
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

            await _storageTemperatureRepository.SaveChangesAsync();

            return preservationType;
        }
      
        public async Task DeletePreservationTypeAsync(PreservationType preservationType)
        {
            await _preservationTypeRepository.DeleteAsync(preservationType.Id);
            await _preservationTypeRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Storage Temperature
        public async Task<StorageTemperature> AddStorageTemperatureAsync(StorageTemperature storageTemperature)
        {
            _storageTemperatureRepository.Insert(storageTemperature);
            await _storageTemperatureRepository.SaveChangesAsync();

            return storageTemperature;
        }

        public async Task<StorageTemperature> UpdateStorageTemperatureAsync(StorageTemperature storageTemperature, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListStorageTemperaturesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                storageTemperature.Value =
                    types
                        .Where(x => x.Id == storageTemperature.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.Id == storageTemperature.Id).First();
            var reverse = (oldType.SortOrder < storageTemperature.SortOrder);

            var newOrder = types
                    .Prepend(storageTemperature)
                    .GroupBy(x => x.Id)
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
                .ForEach(_storageTemperatureRepository.Update);

            await _storageTemperatureRepository.SaveChangesAsync();

            return storageTemperature;
        }

        public async Task DeleteStorageTemperatureAsync(StorageTemperature storageTemperature)
        {
            await _storageTemperatureRepository.DeleteAsync(storageTemperature.Id);
            await _storageTemperatureRepository.SaveChangesAsync();
        }
        #endregion

        #region RefData: Material Type
        public async Task DeleteMaterialTypeAsync(MaterialType materialType)
        {
            await _materialTypeRepository.DeleteAsync(materialType.Id);
            await _materialTypeRepository.SaveChangesAsync();
        }

        public async Task<MaterialType> UpdateMaterialTypeAsync(MaterialType materialType, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListMaterialTypesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                materialType.Value =
                    types
                        .Where(x => x.Id == materialType.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldType = types.Where(x => x.Id == materialType.Id).First();
            var reverse = (oldType.SortOrder < materialType.SortOrder);

            var newOrder = types
                    .Prepend(materialType)
                    .GroupBy(x => x.Id)
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

        #region RefData: Material Type Group
        public async Task DeleteMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup)
        {
            await _materialTypeGroupRepository.DeleteAsync(materialTypeGroup.Id);
            await _materialTypeGroupRepository.SaveChangesAsync();
        }

        public async Task<MaterialTypeGroup> UpdateMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup)
        {
            _materialTypeGroupRepository.Update(materialTypeGroup);
            await _materialTypeRepository.SaveChangesAsync();

            return materialTypeGroup;
        }

        public async Task<MaterialTypeGroup> AddMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup)
        {
            _materialTypeGroupRepository.Insert(materialTypeGroup);
            await _materialTypeGroupRepository.SaveChangesAsync();

            return materialTypeGroup;
        }
        #endregion

        #region RefData: Collection Type
        public async Task DeleteCollectionTypeAsync(CollectionType collectionType)
        {
            await _collectionTypeRepository.DeleteAsync(collectionType.Id);
            await _collectionTypeRepository.SaveChangesAsync();
        }

        public async Task<CollectionType> UpdateCollectionTypeAsync(CollectionType collectionType, bool sortOnly = false)
        {
            var types = await _biobankReadService.ListCollectionTypesAsync();

            // If only updating sortOrder
            if (sortOnly)
            {
                collectionType.Value =
                    types
                        .Where(x => x.Id == collectionType.Id)
                        .First()
                        .Value;
            }

            // Insert respecting Sort Order
            types
                .Prepend(collectionType)          // Add new item
                .GroupBy(x => x.Id) // Remove old item of same ID
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
                serviceOffering.Value =
                    offerings
                        .Where(x => x.Id == serviceOffering.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldStatus = offerings.Where(x => x.Id == serviceOffering.Id).First();
            var reverse = (oldStatus.SortOrder < serviceOffering.SortOrder);

            var newOrder = offerings
                    .Prepend(serviceOffering)
                    .GroupBy(x => x.Id)
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
            await _serviceOfferingRepository.DeleteAsync(serviceOffering.Id);
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
                sex.Value =
                    sexes
                        .Where(x => x.Id == sex.Id)
                        .First()
                        .Value;
            }

            // Add new item, remove old
            var oldType = sexes.Where(x => x.Id == sex.Id).First();
            var reverse = (oldType.SortOrder < sex.SortOrder);

            var newOrder = sexes
                    .Prepend(sex)
                    .GroupBy(x => x.Id)
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
            await _sexRepository.DeleteAsync(sex.Id);
            await _sexRepository.SaveChangesAsync();
        }
        #endregion

        //delete adt
        public async Task DeleteAssociatedDataTypeAsync(AssociatedDataType associatedDataType)
        {
            await _associatedDataTypeRepository.DeleteAsync(associatedDataType.Id);
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
            await _countyRepository.DeleteAsync(county.Id);
            await _countyRepository.SaveChangesAsync();
        }

        #endregion
        
        public async Task DeleteRegistrationReasonAsync(RegistrationReason registrationReason)
        {
            await _registrationReasonRepository.DeleteAsync(registrationReason.Id);
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

        public async Task UpdateOrganisationURLAsync(int id)
        {
            var biobank = await _organisationRepository.GetByIdAsync(id);
      
            //Transform the URL
            biobank.Url = UrlTransformer.Transform(biobank.Url);

            //Update
            _organisationRepository.Update(biobank);
            
            await _organisationRepository.SaveChangesAsync();
         
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
            await _indexService.BulkDeleteBiobank(id);
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
    
        public async Task DeleteAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup)
        {
            await _associatedDataTypeGroupRepository.DeleteAsync(associatedDataTypeGroup.Id);
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
            await _annualStatisticGroupRepository.DeleteAsync(annualStatisticGroup.Id);
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

        public async Task<KeyValuePair<string,string>> GenerateNewApiClientForBiobank(int biobankId, string clientName=null)
        {
            var clientId = Crypto.GenerateId();
            var clientSecret = Crypto.GenerateId();
            (await _organisationRepository.GetByIdAsync(biobankId)).ApiClients.Add(new ApiClient
            {
                Name = clientName ?? clientId,
                ClientId = clientId,
                ClientSecretHash = clientSecret.Sha256()
            });

            await _organisationRepository.SaveChangesAsync();
            return new KeyValuePair<string, string> (clientId,clientSecret);
        }

        public async Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId)
        {
            //Generates and update biobank api client with new client secret.
            var biobank = await _organisationRepository.GetByIdAsync(biobankId);

            var newSecret = Crypto.GenerateId();
            var credentials = biobank.ApiClients.First();
            credentials.ClientSecretHash = newSecret.Sha256();

            await _organisationRepository.SaveChangesAsync();
            return new KeyValuePair<string, string>(credentials.ClientId, newSecret);

        }
    }
}
