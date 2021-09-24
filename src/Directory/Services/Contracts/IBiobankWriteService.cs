using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Dto;

namespace Biobanks.Services.Contracts
{
    public interface IBiobankWriteService
    {
        Task AddSampleSetAsync(SampleSet sampleSet);
        Task UpdateSampleSetAsync(SampleSet sampleSet);
        Task DeleteSampleSetAsync(int id);

        Task AddCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task UpdateCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task DeleteCapabilityAsync(int id);

        Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference);
        Task RemoveLogoAsync(int organisationId);

        Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork);

        Task<Organisation> CreateBiobankAsync(OrganisationDTO biobank);
        Task<Organisation> UpdateBiobankAsync(OrganisationDTO biobank);
        Task<OrganisationUser> AddUserToBiobankAsync(string userId, int biobankId);
        Task RemoveUserFromBiobankAsync(string userId, int biobankId);

        Task<OrganisationRegisterRequest> AddRegisterRequestAsync(OrganisationRegisterRequest request);
        Task DeleteRegisterRequestAsync(OrganisationRegisterRequest request);

        Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services);
        Task DeleteBiobankServiceAsync(int biobankId, int serviceId);

        Task<Network> CreateNetworkAsync(Network network);
        Task<Network> UpdateNetworkAsync(NetworkDTO networkDto);
        Task<NetworkUser> AddUserToNetworkAsync(string userId, int networkId);
        Task RemoveUserFromNetworkAsync(string userId, int networkId);

        Task<NetworkRegisterRequest> AddNetworkRegisterRequestAsync(NetworkRegisterRequest request);
        Task DeleteNetworkRegisterRequestAsync(NetworkRegisterRequest request);
        Task<bool> AddBiobankToNetworkAsync(int biobankId, int networkId, string biobankExternalID, bool approve);
        Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId);

        Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request);
        Task<NetworkRegisterRequest> UpdateNetworkRegisterRequestAsync(NetworkRegisterRequest request);

        Task DeleteOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> UpdateOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm diagnosis);
        Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);
        Task UpdateOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);

        Task<Organisation> SuspendBiobankAsync(int id);
        Task<Organisation> UnsuspendBiobankAsync(int id);
        Task UpdateOrganisationURLAsync(int id);
        Task<bool> AddFunderToBiobankAsync(int funderId, int biobankId);
        Task RemoveFunderFromBiobankAsync(int funderId, int biobankId);
        Task DeleteBiobankAsync(int id);

        Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year);
        Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons);
        Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId);

        Task<KeyValuePair<string, string>> GenerateNewApiClientForBiobank(int biobankId, string clientName = null);
        Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int biobankId);
    }
}
