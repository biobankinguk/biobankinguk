using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Dto;

namespace Biobanks.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id, bool onlyDisplayable = false);
        Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email);

        Task<NetworkRegisterRequest> GetNetworkRegisterRequestByUserEmailAsync(string email);

        Task<Organisation> GetBiobankByIdAsync(int biobankId);
        Task<Organisation> GetBiobankByIdForIndexingAsync(int biobankId);
        Task<Organisation> GetBiobankByNameAsync(string biobankName);
        Task<Blob> GetLogoBlobAsync(string logoName);
        
        Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync();

        Task<SampleSet> GetSampleSetByIdAsync(int id);
        Task<SampleSet> GetSampleSetByIdForIndexingAsync(int id);
        bool CanThisBiobankAdministerThisCollection(int biobankId, int collectionId);
        bool CanThisBiobankAdministerThisSampleSet(int biobankId, int sampleSetId);

        Task<DiagnosisCapability> GetCapabilityByIdAsync(int id);
        Task<DiagnosisCapability> GetCapabilityByIdForIndexingAsync(int id);
        Task<IEnumerable<int>> GetCapabilityIdsByOntologyTermAsync(string ontologyTerm);
        Task<IEnumerable<DiagnosisCapability>> ListCapabilitiesAsync(int organisationId);
        bool CanThisBiobankAdministerThisCapability(int biobankId, int capabilityId);

        IEnumerable<string> ExtractDistinctMaterialTypes(Collection collection);

        Task<IEnumerable<AssociatedDataType>> ListAssociatedDataTypesAsync();
        Task<IEnumerable<AssociatedDataTypeGroup>> ListAssociatedDataTypeGroupsAsync(string wildcard = "");
        Task<int> GetAssociatedDataTypeGroupCount(int associatedDataTypeGroupId);
        Task<bool> IsAssociatedDataTypeGroupInUse(int associatedDataTypeGroupId);
        Task<bool> ValidAssociatedDataTypeGroupNameAsync(int associatedDataTypeGroupId, string associatedDataTypeGroupName);
        Task<bool> ValidAssociatedDataTypeGroupNameAsync(string associatedDataTypeGroupName);

        Task<bool> IsMaterialTypeAssigned(int id);
        Task<IEnumerable<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeFrames();
        Task<int> GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(int id);
        Task<bool> IsAssociatedDataProcurementTimeFrameInUse(int id);
        Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(int procurementId, string procurementDescription);
        Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(string procurementDescription);

        Task<IEnumerable<OntologyTerm>> SearchOntologyTerms(string description = null, List<string> tags = null);
        Task<IEnumerable<OntologyTerm>> ListOntologyTerms(string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(int start, int length, string description = null, List<string> tags = null);
        Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null);
        Task<bool> IsOntologyTermInUse(string id);
        Task<int> CountOntologyTerms(string description = null, List<string> tags = null);
        Task<int> GetOntologyTermCollectionCapabilityCount(string id);

        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);
        Task<bool> IsExtractionProcedureInUse(string id);

        Task<IEnumerable<SnomedTag>> ListSnomedTags();
        Task<SnomedTag> GetSnomedTagByDescription(string description);

        Task<int> GetAssociatedDataTypeCollectionCapabilityCount(int id);
        Task<bool> ValidAssociatedDataTypeDescriptionAsync(string associatedDataTypeDescription);
        Task<bool> ValidAssociatedDataTypeDescriptionAsync(int associatedDataTypeId, string associatedDataTypeDescription);

        Task<int> GetMaterialTypeMaterialDetailCount(int id);

        Task<int> GetServiceOfferingOrganisationCount(int id);

        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId);

        Task<IEnumerable<ApplicationUser>> ListBiobankAdminsAsync(int biobankId);
        Task<OrganisationType> GetBiobankOrganisationTypeAsync();


        Task<Network> GetNetworkByIdAsync(int networkId);
        Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId);
        Task<Network> GetNetworkByNameAsync(string networkName);

        Task<IEnumerable<Organisation>> ListBiobanksAsync(string wildcard = "", bool includeSuspended = true);
        Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdAsync(int networkId);
        Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdForIndexingAsync(int networkId);
        Task<IEnumerable<Network>> ListNetworksAsync();
        Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync();
        Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync();
        Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedNetworkRegisterRequestAsync();
        Task<IEnumerable<NetworkRegisterRequest>> ListOpenNetworkRegisterRequestsAsync();

        Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync();
        Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalNetworkRegisterRequestsAsync();

        Task<OrganisationRegisterRequest> GetBiobankRegisterRequestAsync(int requestId);
        Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByOrganisationNameAsync(string name);
        Task<NetworkRegisterRequest> GetNetworkRegisterRequestAsync(int requestId);
        Task<Organisation> GetBiobankByExternalIdAsync(string externalId);
        Task<Organisation> GetBiobankByExternalIdForSearchResultsAsync(string externalId);
        Task<IEnumerable<Organisation>> GetBiobanksByExternalIdsAsync(IList<string> biobankExternalIds);
        Task<IEnumerable<Network>> GetNetworksByBiobankIdAsync(int organisationId);

        Task<bool> IsAssociatedDataTypeInUse(int id);

        Task<IEnumerable<int>> GetAllSampleSetIdsAsync();
        Task<IEnumerable<int>> GetAllCapabilityIdsAsync();

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexingAsync(IEnumerable<int> sampleSetIds);
        Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexingAsync(IEnumerable<int> capabilityIds);
        Task<bool> BiobankRegisterRequestExists(string name);
        Task<bool> NetworkRegisterRequestExists(string name);

        /// <summary>
        /// Gets a count of all Sample Sets in the database.
        /// </summary>
        /// <returns>A count of all Sample Sets in the database.</returns>
        Task<int> GetSampleSetCountAsync();
        /// <summary>
        /// Gets a count of all Capabilities in the database.
        /// </summary>
        /// <returns>A count of all Capabilities in the database.</returns>
        Task<int> GetCapabilityCountAsync();

        Task<IEnumerable<SampleSet>> GetSampleSetsByIdsForIndexDeletionAsync(IEnumerable<int> sampleSetIds);
        Task<IEnumerable<DiagnosisCapability>> GetCapabilitiesByIdsForIndexDeletionAsync(IEnumerable<int> capabilityIds);
        Task<int> GetIndexableSampleSetCountAsync();
        Task<int> GetIndexableCapabilityCountAsync();
        Task<int> GetSuspendedSampleSetCountAsync();
        Task<int> GetSuspendedCapabilityCountAsync();
        Task<Dictionary<int,string>> GetDescriptionsByCollectionIds(IEnumerable<int> collectionIds);

        Task<bool> IsBiobankSuspendedAsync(int biobankId);
        Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId);
        Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId);
        Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId);

        Task<IEnumerable<Funder>> ListBiobankFundersAsync(int biobankId);

        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int modelBiobankId);
        List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId);

        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId);


        Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers);
        Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId);

        List<Organisation> GetOrganisations();

        Task<bool> OrganisationIncludesPublications(int biobankId);
        Task<string> GetUnusedTokenByUser(string biobankUserId);
        Task<bool> IsBiobankAnApiClient(int biobankId);
    }
}
