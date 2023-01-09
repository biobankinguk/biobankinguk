using AutoMapper;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services;
using Biobanks.Submissions.Api.Models.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankWriteService : IBiobankWriteService
    {
        #region Properties and ctor
        private readonly IOntologyTermService _ontologyTermService;
        private readonly ICapabilityService _capabilityService;
        private readonly IOrganisationDirectoryService _organisationService;

        private readonly ILogoStorageProvider _logoStorageProvider;

        private readonly IGenericEFRepository<OntologyTerm> _ontologyTermRepository;
        private readonly IGenericEFRepository<MaterialType> _materialTypeRepository;

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

        private readonly IGenericEFRepository<ConsentRestriction> _consentRestrictionRepository;

        private readonly IGenericEFRepository<Funder> _funderRepository;

        private readonly IGenericEFRepository<Entities.Data.Config> _siteConfigRepository;

        private readonly IBiobankIndexService _indexService;

        private readonly IMapper _mapper;

        public BiobankWriteService(
            ICollectionService collectionService,
            IOntologyTermService ontologyTermService,
            ICapabilityService capabilityService,
            INetworkService networkService,
            IOrganisationDirectoryService organisationService,
            IBiobankReadService biobankReadService,
            IConfigService configService,
            ILogoStorageProvider logoStorageProvider,
            IGenericEFRepository<OntologyTerm> ontologyTermRepository,
            IGenericEFRepository<MaterialType> materialTypeRepository,
            IGenericEFRepository<Collection> collectionRepository,
            IGenericEFRepository<DiagnosisCapability> capabilityRepository,
            IGenericEFRepository<SampleSet> sampleSetRepository,
            IGenericEFRepository<MaterialDetail> materialDetailRepository,
            IGenericEFRepository<Network> networkRepository,
            IGenericEFRepository<NetworkUser> networkUserRepository,
            IGenericEFRepository<NetworkRegisterRequest> networkRegisterRequestRepository,
            IGenericEFRepository<OrganisationNetwork> networkOrganisationRepository,

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

            IGenericEFRepository<ConsentRestriction> consentRestrictionRepository,

            IGenericEFRepository<Entities.Data.Config> siteConfigRepository,

            IBiobankIndexService indexService,
            IMapper mapper,

            IGenericEFRepository<Funder> funderRepository)
        {
            _ontologyTermService = ontologyTermService;
            _capabilityService = capabilityService;

            _organisationService = organisationService;

            _logoStorageProvider = logoStorageProvider;

            _ontologyTermRepository = ontologyTermRepository;
            _materialTypeRepository = materialTypeRepository;

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

            _consentRestrictionRepository = consentRestrictionRepository;

            _siteConfigRepository = siteConfigRepository;

            _indexService = indexService;

            _mapper = mapper;
            _funderRepository = funderRepository;
        }

        #endregion
       
        


        public async Task AddCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
            var ontologyTerm = await _ontologyTermService.Get(value: capabilityDTO.OntologyTerm, onlyDisplayable: true);

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

            if (!await _organisationService.IsSuspended(capability.OrganisationId))
                await _indexService.IndexCapability(capability.DiagnosisCapabilityId);
        }

        public async Task UpdateCapabilityAsync(CapabilityDTO capabilityDTO, IEnumerable<CapabilityAssociatedData> associatedData)
        {
            var existingCapability = (await _capabilityRepository.ListAsync(true,
                x => x.DiagnosisCapabilityId == capabilityDTO.Id,
                null,
                x => x.AssociatedData)).First();

            existingCapability.AssociatedData.Clear();

            var ontologyTerm = await _ontologyTermService.Get(value: capabilityDTO.OntologyTerm, onlyDisplayable: true);

            existingCapability.OntologyTermId = ontologyTerm.Id;
            existingCapability.AnnualDonorExpectation = capabilityDTO.AnnualDonorExpectation.Value;
            existingCapability.SampleCollectionModeId = capabilityDTO.SampleCollectionModeId;
            existingCapability.LastUpdated = DateTime.Now;

            existingCapability.AssociatedData = associatedData.ToList();

            await _capabilityRepository.SaveChangesAsync();

            if (!await _organisationService.IsSuspended(existingCapability.OrganisationId))
                await _capabilityService.UpdateCapabilityDetails(existingCapability.DiagnosisCapabilityId);
        }

        public async Task DeleteCapabilityAsync(int id)
        {
            var capability = await _capabilityRepository.GetByIdAsync(id);
            await _capabilityRepository.DeleteWhereAsync(x => x.DiagnosisCapabilityId == id);

            await _capabilityRepository.SaveChangesAsync();

            if (!await _organisationService.IsSuspended(capability.OrganisationId))
                _indexService.DeleteCapability(id);
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
            var organisation = await _organisationRepository.GetByIdAsync(id);

            // Remove From Index
            _indexService.BulkDeleteBiobank(organisation);

            // Delete Organisation
            await _organisationRepository.DeleteAsync(id);
            await _organisationRepository.SaveChangesAsync();
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

    }
}
