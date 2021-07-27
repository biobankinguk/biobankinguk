using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Biobanks.Search.Constants;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Dto;

namespace Biobanks.Services.Contracts
{
    public interface IBiobankReadService
    {
        Task<IEnumerable<OntologyTerm>> GetMaterialTypeExtractionProcedures(int id);
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
        Task<IEnumerable<AccessCondition>> ListAccessConditionsAsync();
        Task<IEnumerable<CollectionType>> ListCollectionTypesAsync();
        Task<IEnumerable<CollectionStatus>> ListCollectionStatusesAsync();

        Task<IEnumerable<Sex>> ListSexesAsync();
        Task<IEnumerable<AssociatedDataType>> ListAssociatedDataTypesAsync();
        Task<IEnumerable<AssociatedDataTypeGroup>> ListAssociatedDataTypeGroupsAsync(string wildcard = "");
        Task<int> GetAssociatedDataTypeGroupCount(int associatedDataTypeGroupId);
        Task<bool> IsAssociatedDataTypeGroupInUse(int associatedDataTypeGroupId);
        Task<bool> ValidAssociatedDataTypeGroupNameAsync(int associatedDataTypeGroupId, string associatedDataTypeGroupName);
        Task<bool> ValidAssociatedDataTypeGroupNameAsync(string associatedDataTypeGroupName);
        
        Task<IEnumerable<MaterialType>> ListMaterialTypesAsync();
        Task<bool> IsMaterialTypeAssigned(int id);

        Task<IEnumerable<MaterialTypeGroup>> ListMaterialTypeGroupsAsync();
        Task<bool> ValidMaterialTypeGroupDescriptionAsync(string materialTypeDescription);
        Task<bool> IsMaterialTypeGroupInUse(int id);


        Task<IEnumerable<AgeRange>> ListAgeRangesAsync();
        Task<bool> ValidAgeRangeAsync(string ageRangeDescription);
        Task<bool> IsAgeRangeDescriptionInUse(int ageRangeId, string ageRangeDescription);
        Task<bool> IsAgeRangeInUse(int id);
        Task<int> GetAgeRangeUsageCount(int id);

        Task<bool> AreAgeRangeBoundsNull(int id);

        Task<IEnumerable<AnnualStatistic>> ListAnnualStatisticsAsync();
        Task<bool> ValidAnnualStatisticAsync(string annualStatisticDescription, int annualStatisticGroupId);
        Task<bool> IsAnnualStatisticInUse(int id);
        Task<int> GetAnnualStatisticUsageCount(int id);

        Task<IEnumerable<CollectionPercentage>> ListCollectionPercentagesAsync();
        Task<bool> ValidCollectionPercentageAsync(string collectionPercentageDescription);
        Task<bool> IsCollectionPercentageInUse(int id);
        Task<int> GetCollectionPercentageUsageCount(int id);

        Task<IEnumerable<DonorCount>> ListDonorCountsAsync(bool ignoreCache = false);
        Task<int> GetDonorCountUsageCount(int id);
        Task<bool> ValidDonorCountAsync(string donorCountDescription);
        Task<bool> IsDonorCountInUse(int id);

        Task<int> GetRegistrationReasonOrganisationCount(int id);
        Task<bool> IsRegistrationReasonInUse(int id);
        Task<bool> ValidRegistrationReasonDescriptionAsync(int reasonId, string reasonDescription);
        Task<bool> ValidRegistrationReasonDescriptionAsync(string reasonDescription);

        Task<int> GetCollectionStatusCollectionCount(int id);
        Task<bool> IsCollectionStatusInUse(int id);
        Task<bool> ValidCollectionStatusDescriptionAsync(string collectionStatusDescription);
        Task<bool> ValidCollectionStatusDescriptionAsync(int collectionStatusId, string collectionStatusDescription);

        Task<int> GetCollectionTypeCollectionCount(int id);
        Task<bool> IsCollectionTypeInUse(int id);
        Task<bool> ValidCollectionTypeDescriptionAsync(string collectionTypeDescription);
        Task<bool> ValidCollectionTypeDescriptionAsync(int collectionTypeId, string collectionTypeDescription);

        Task<IEnumerable<PreservationType>> ListPreservationTypesAsync();
        Task<int> GetPreservationTypeUsageCount(int id);
        Task<bool> IsPreservationTypeInUse(int id);
        Task<bool> ValidPreservationTypeAsync(string value, int? storageTemperatureId);

        Task<IEnumerable<StorageTemperature>> ListStorageTemperaturesAsync();
        Task<bool> ValidStorageTemperatureAsync(string preservationTypeDescription);
        Task<bool> IsStorageTemperatureInUse(int id);
        Task<bool> IsStorageTemperatureAssigned(int id);
        Task<int> GetStorageTemperatureUsageCount(int id);

        Task<IEnumerable<SopStatus>> ListSopStatusesAsync();
        Task<bool> ValidSopStatusAsync(string sopStatusDescription);
        Task<bool> IsSopStatusInUse(int id);
        Task<int> GetSopStatusUsageCount(int id);

        Task<IEnumerable<MacroscopicAssessment>> ListMacroscopicAssessmentsAsync();
        Task<bool> ValidMacroscopicAssessmentAsync(string macroscopicAssessmentDescription);
        Task<bool> IsMacroscopicAssessmentInUse(int id);
        Task<int> GetMacroscopicAssessmentUsageCount(int id);

        Task<IEnumerable<SampleCollectionMode>> ListSampleCollectionModeAsync();
        Task<bool> ValidSampleCollectionModeAsync(string sampleCollectionMode);
        Task<bool> IsSampleCollectionModeInUse(int id);
        Task<int> GetSampleCollectionModeUsageCount(int id);

        Task<IEnumerable<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeFrames();
        Task<int> GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(int id);
        Task<bool> IsAssociatedDataProcurementTimeFrameInUse(int id);
        Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(int procurementId, string procurementDescription);
        Task<bool> ValidAssociatedDataProcurementTimeFrameDescriptionAsync(string procurementDescription);

        Task<IEnumerable<OntologyTerm>> ListOntologyTerms(string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(int start, int length, string description = null, List<string> tags = null);
        Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null);
        Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null);
        Task<bool> IsOntologyTermInUse(string id);
        Task<int> CountOntologyTerms(string description = null, List<string> tags = null);
        Task<int> GetOntologyTermCollectionCapabilityCount(string id);

        Task<IEnumerable<OntologyTerm>> ListExtractionProceduresAsync(string wildcard = "");
        Task<OntologyTerm> GetExtractionProcedureById(string id);
        Task<int> GetExtractionProcedureMaterialDetailsCount(string id);
        Task<bool> IsExtractionProcedureInUse(string id);

        Task<IEnumerable<SnomedTag>> ListSnomedTags();
        Task<SnomedTag> GetSnomedTagByDescription(string description);

        Task<int> GetAssociatedDataTypeCollectionCapabilityCount(int id);
        Task<bool> ValidAssociatedDataTypeDescriptionAsync(string associatedDataTypeDescription);
        Task<bool> ValidAssociatedDataTypeDescriptionAsync(int associatedDataTypeId, string associatedDataTypeDescription);

        Task<int> GetCountryCountyOrganisationCount(int id);
        Task<bool> ValidCountryNameAsync(string countryName);
        Task<bool> ValidCountryNameAsync(int countryId, string countryName);

        Task<int> GetAccessConditionsCount(int id);
        Task<bool> ValidAccessConditionDescriptionAsync(string accessConditionsDescription);
        Task<bool> ValidAccessConditionDescriptionAsync(int accessConditionsId, string accessConditionsDescription);

        Task<int> GetMaterialTypeMaterialDetailCount(int id);
        Task<bool> ValidMaterialTypeDescriptionAsync(string materialTypeDescription);
        Task<bool> ValidMaterialTypeDescriptionAsync(int materialTypeId, string materialTypeDescription);

        Task<IEnumerable<ConsentRestriction>> ListConsentRestrictionsAsync();
        Task<int> GetConsentRestrictionCollectionCount(int id);
        Task<bool> ValidConsentRestrictionDescriptionAsync(string consentDescription);
        Task<bool> ValidConsentRestrictionDescriptionAsync(int consentId, string consentDescription);

        Task<int> GetServiceOfferingOrganisationCount(int id);
        Task<bool> ValidServiceOfferingName(int offeringId, string offeringName);
        Task<bool> ValidServiceOfferingName(string offeringName);

        Task<int> GetSexCount(int id);
        Task<bool> ValidSexDescriptionAsync(string sexDescription);
        Task<bool> ValidSexDescriptionAsync(int sexId, string sexDescription);

        Task<IEnumerable<OrganisationServiceOffering>> ListBiobankServiceOfferingsAsync(int biobankId);
        Task<IEnumerable<ServiceOffering>> ListServiceOfferingsAsync();

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

        Task<bool> IsMaterialTypeInUse(int id);
        Task<bool> IsAssociatedDataTypeInUse(int id);
        Task<bool> IsSexInUse(int id);
        Task<bool> IsServiceOfferingInUse(int id);
        Task<bool> IsConsentRestrictionInUse(int id);
        Task<bool> IsCountryInUse(int id);
        Task<bool> IsAccessConditionInUse(int id);

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
        Task<Funder> GetFunderbyName(string name);
        Task<IEnumerable<Funder>>  ListFundersAsync(string wildcard);

        Task<ICollection<County>> ListCountiesAsync();
        Task<bool> ValidCountyAsync(string countyName);
        Task<bool> IsCountyInUse(int id);
        Task<int> GetCountyUsageCount(int id);

        Task<ICollection<Country>> ListCountriesAsync();
        Task<IEnumerable<ApplicationUser>> ListSoleBiobankAdminIdsAsync(int modelBiobankId);
        Task<Funder> GetFunderByIdAsync(int id);
        List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId);
        List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId);

        Task<IEnumerable<AnnualStatisticGroup>> GetAnnualStatisticGroupsAsync();

        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId);
        Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId);


        Task<IEnumerable<Organisation>> GetBiobanksByAnonymousIdentifiersAsync(IEnumerable<Guid> biobankAnonymousIdentifiers);
        Task<IEnumerable<RegistrationReason>> ListRegistrationReasonsAsync();
        Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId);

        Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId);
        Task<IEnumerable<Publication>> GetOrganisationPublicationsAsync(Organisation organisation);
        Task<IEnumerable<Publication>> GetAcceptedOrganisationPublicationsAsync(Organisation organisation);
        List<Organisation> GetOrganisations();

        Task<AnnualStatisticGroup> GetAnnualStatisticGroupByName(string name);
        Task<int> GetAnnualStatisticAnnualStatisticGroupCount(int annualStatisticGroupId);
        Task<bool> IsAnnualStatisticGroupInUse(int annualStatisticGroupId);
        Task<IEnumerable<AnnualStatisticGroup>> ListAnnualStatisticGroupsAsync(string wildcard = "");
        Task<bool> ValidAnnualStatisticGroupNameAsync(int annualStatisticGroupId, string annualStatisticGroupName);
        Task<bool> ValidAnnualStatisticGroupNameAsync(string annualStatisticGroupName);
        Task<bool> OrganisationIncludesPublications(int biobankId);
        Task<string> GetUnusedTokenByUser(string biobankUserId);
        Task<bool> IsBiobankAnApiClient(int biobankId);
    }
}
