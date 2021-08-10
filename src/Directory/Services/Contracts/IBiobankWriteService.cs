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
        Task<Collection> AddCollectionAsync(Collection collection, string ontologyTermDescription, 
            IEnumerable<CollectionAssociatedData> associatedData, IEnumerable<int> consentRestrictionIds);
        Task UpdateCollectionAsync(Collection collection, string ontologyTermDescription,
            IEnumerable<CollectionAssociatedData> associatedData, IEnumerable<int> consentRestrictionIds);
        Task<bool> DeleteCollectionAsync(int id);

        Task AddSampleSetAsync(SampleSet sampleSet);
        Task UpdateSampleSetAsync(SampleSet sampleSet);
        Task DeleteSampleSetAsync(int id);

        Task AddCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task UpdateCapabilityAsync(CapabilityDTO capability, IEnumerable<CapabilityAssociatedData> associatedData);
        Task DeleteCapabilityAsync(int id);

        Task DeleteFunderByIdAsync(int id);
        Task<Funder> AddFunderAsync(Funder funder);
        Task<Funder> UpdateFunderAsync(Funder funder);

        Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference);
        Task RemoveLogoAsync(int organisationId);

        Task DeleteAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe);
        Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe, bool sortOnly = false);
        Task<AssociatedDataProcurementTimeframe> AddAssociatedDataProcurementTimeFrameAsync(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe);

        Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services);
        Task DeleteBiobankServiceAsync(int biobankId, int serviceId);
        Task<ServiceOffering> UpdateServiceOfferingAsync(ServiceOffering serviceOffering, bool sortOnly = false);
        Task DeleteServiceOfferingAsync(ServiceOffering serviceOffering);

        Task DeleteOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> UpdateOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm diagnosis);
        Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);
        Task UpdateOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);

        Task<MaterialType> AddMaterialTypeAsync(MaterialType materialType);
        Task<MaterialType> UpdateMaterialTypeAsync(MaterialType materialType, bool sortOnly = false);
        Task DeleteMaterialTypeAsync(MaterialType materialType);

        Task<MaterialTypeGroup> AddMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup);
        Task<MaterialTypeGroup> UpdateMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup);
        Task DeleteMaterialTypeGroupAsync(MaterialTypeGroup materialTypeGroup);

        Task<Sex> AddSexAsync(Sex sex);
        Task<Sex> UpdateSexAsync(Sex sex, bool sortOnly = false);
        Task DeleteSexAsync(Sex sex);

        Task<AgeRange> AddAgeRangeAsync(AgeRange ageRange);
        Task<AgeRange> UpdateAgeRangeAsync(AgeRange ageRange, bool sortOnly = false);
        Task DeleteAgeRangeAsync(AgeRange ageRange);

        Task<AnnualStatistic> AddAnnualStatisticAsync(AnnualStatistic annualStatistic);
        Task<AnnualStatistic> UpdateAnnualStatisticAsync(AnnualStatistic annualStatistic, bool sortOnly = false);
        Task DeleteAnnualStatisticAsync(AnnualStatistic annualStatistic);

        Task DeleteRegistrationReasonAsync(RegistrationReason registrationReason);
        Task<RegistrationReason> UpdateRegistrationReasonAsync(RegistrationReason registrationReason);
        Task<RegistrationReason> AddRegistrationReasonAsync(RegistrationReason registrationReason);

        Task<CollectionPercentage> AddCollectionPercentageAsync(CollectionPercentage collectionPercentage);
        Task<CollectionPercentage> UpdateCollectionPercentageAsync(CollectionPercentage collectionPercentage, bool sortOnly = false);
        Task DeleteCollectionPercentageAsync(CollectionPercentage collectionPercentage);

        Task<DonorCount> AddDonorCountAsync(DonorCount donorCount);
        Task<DonorCount> UpdateDonorCountAsync(DonorCount donorCount, bool sortOnly = false);
        Task DeleteDonorCountAsync(DonorCount donorCount);

        Task<SopStatus> AddSopStatusAsync(SopStatus sopStatus);
        Task<SopStatus> UpdateSopStatusAsync(SopStatus sopStatus, bool sortOnly = false);
        Task DeleteSopStatusAsync(SopStatus sopStatus);

        Task DeleteCollectionStatusAsync(CollectionStatus collectionStatus);
        Task<CollectionStatus> UpdateCollectionStatusAsync(CollectionStatus collectionStatus, bool sortOnly = false);
        Task<CollectionStatus> AddCollectionStatusAsync(CollectionStatus collectionStatus);

        Task DeleteCollectionTypeAsync(CollectionType collectionType);
        Task<CollectionType> UpdateCollectionTypeAsync(CollectionType collectionType, bool sortOnly = false);
        Task<CollectionType> AddCollectionTypeAsync(CollectionType collectionType);

        Task<MacroscopicAssessment> AddMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment);
        Task<MacroscopicAssessment> UpdateMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment, bool sortOnly = false);
        Task DeleteMacroscopicAssessmentAsync(MacroscopicAssessment macroscopicAssessment);

        Task<SampleCollectionMode> AddSampleCollectionModeAsync(SampleCollectionMode sampleCollection);
        Task<SampleCollectionMode> UpdateSampleCollectionModeAsync(SampleCollectionMode sampleCollection, bool sortOnly = false);
        Task DeleteSampleCollectionModeAsync(SampleCollectionMode sampleCollection);

        Task<PreservationType> AddPreservationTypeAsync(PreservationType preservationType);
        Task<PreservationType> UpdatePreservationTypeAsync(PreservationType preservationType, bool sortOnly = false);
        Task DeletePreservationTypeAsync(PreservationType preservationType);

        Task<StorageTemperature> AddStorageTemperatureAsync(StorageTemperature storageTemperature);
        Task<StorageTemperature> UpdateStorageTemperatureAsync(StorageTemperature storageTemperature, bool sortOnly=false);
        Task DeleteStorageTemperatureAsync(StorageTemperature storageTemperature);

        Task DeleteAssociatedDataTypeAsync(AssociatedDataType associatedDataType);
        Task<AssociatedDataType> UpdateAssociatedDataTypeAsync(AssociatedDataType associatedDataType);
        Task<AssociatedDataType> AddAssociatedDataTypeAsync(AssociatedDataType associatedDataType);

        Task DeleteAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup);
        Task<AssociatedDataTypeGroup> AddAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup);
        Task<AssociatedDataTypeGroup> UpdateAssociatedDataTypeGroupAsync(AssociatedDataTypeGroup associatedDataTypeGroup);

        Task<ConsentRestriction> AddConsentRestrictionAsync(ConsentRestriction consentRestriction);
        Task DeleteConsentRestrictionAsync(ConsentRestriction consentRestriction);
        Task<ConsentRestriction> UpdateConsentRestrictionAsync(ConsentRestriction consentRestriction, bool sortOnly=false);

        Task<AccessCondition> AddAccessConditionAsync(AccessCondition accessCondition);
        Task<AccessCondition> UpdateAccessConditionAsync(AccessCondition accessCondition, bool sortOnly = false);
        Task DeleteAccessConditionAsync(AccessCondition accessCondition);

        Task<ServiceOffering> AddServiceOfferingAsync(ServiceOffering serviceOffering);

        Task<Country> AddCountryAsync(Country country);
        Task<Country> UpdateCountryAsync(Country country);
        Task DeleteCountryAsync(Country country);

        Task<County> AddCountyAsync(County county);
        Task<County> UpdateCountyAsync(County county);
        Task DeleteCountyAsync(County county);

        Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year);
        Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons);
        Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId);

        Task<Publication> AddOrganisationPublicationAsync(Publication publication);
        Task<Publication> UpdateOrganisationPublicationAsync(Publication publication);
        Task<AnnualStatisticGroup> AddAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup);
        Task DeleteAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup);
        Task<AnnualStatisticGroup> UpdateAnnualStatisticGroupAsync(AnnualStatisticGroup annualStatisticGroup);
    }
}
