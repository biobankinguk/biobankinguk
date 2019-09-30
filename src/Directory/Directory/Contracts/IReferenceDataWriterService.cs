using Common.Data.ReferenceData;
using Common.DTO;
using System.Threading.Tasks;

namespace Directory.Contracts
{
    /// <summary>
    /// Service class for writing, editing and deleting Reference Data entities to/from the DB.
    /// </summary>
    public interface IReferenceDataWriterService
    {
        Task<AccessCondition> CreateAccessCondition(SortedRefDataBaseDto accessCondition);
        Task<AccessCondition> UpdateAccessCondition(int id, SortedRefDataBaseDto accessCondition);
        Task DeleteAccessCondition(int id);
        Task<AgeRange> CreateAgeRange(SortedRefDataBaseDto ageRange);
        Task<AgeRange> UpdateAgeRange(int id, SortedRefDataBaseDto ageRange);
        Task DeleteAgeRange(int id);
        Task<AnnualStatistic> CreateAnnualStatistic(RefDataBaseDto annualStatistic);
        Task<AnnualStatistic> UpdateAnnualStatistic(int id, RefDataBaseDto annualStatistic);
        Task DeleteAnnualStatistic(int id);
        Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(SortedRefDataBaseDto associatedDataProcurementTimeframe);
        Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(int id, SortedRefDataBaseDto associatedDataProcurementTimeframe);
        Task DeleteAssociatedDataProcurementTimeframe(int id);
        Task<AssociatedDataType> CreateAssociatedDataType(RefDataBaseDto associatedDataType);
        Task<AssociatedDataType> UpdateAssociatedDataType(int id, RefDataBaseDto associatedDataType);
        Task DeleteAssociatedDataType(int id);
        Task<CollectionPercentage> CreateCollectionPercentage(SortedRefDataBaseDto collectionPercentage);
        Task<CollectionPercentage> UpdateCollectionPercentage(int id, SortedRefDataBaseDto collectionPercentage);
        Task DeleteCollectionPercentage(int id);
        Task<CollectionPoint> CreateCollectionPoint(SortedRefDataBaseDto collectionPoint);
        Task<CollectionPoint> UpdateCollectionPoint(int id, SortedRefDataBaseDto collectionPoint);
        Task DeleteCollectionPoint(int id);
        Task<CollectionStatus> CreateCollectionStatus(SortedRefDataBaseDto collectionStatus);
        Task<CollectionStatus> UpdateCollectionStatus(int id, SortedRefDataBaseDto collectionStatus);
        Task DeleteCollectionStatus(int id);
        Task<CollectionType> CreateCollectionType(SortedRefDataBaseDto collectionType);
        Task<CollectionType> UpdateCollectionType(int id, SortedRefDataBaseDto collectionType);
        Task DeleteCollectionType(int id);
        Task<ConsentRestriction> CreateConsentRestriction(SortedRefDataBaseDto consentRestriction);
        Task<ConsentRestriction> UpdateConsentRestriction(int id, SortedRefDataBaseDto consentRestriction);
        Task DeleteConsentRestriction(int id);
        Task<Country> CreateCountry(RefDataBaseDto country);
        Task<Country> UpdateCountry(int id, RefDataBaseDto country);
        Task DeleteCountry(int id);
        Task<County> CreateCounty(RefDataBaseDto county);
        Task<County> UpdateCounty(int id, RefDataBaseDto county);
        Task DeleteCounty(int id);
        Task<DonorCount> CreateDonorCount(SortedRefDataBaseDto donorCount);
        Task<DonorCount> UpdateDonorCount(int id, SortedRefDataBaseDto donorCount);
        Task DeleteDonorCount(int id);
        Task<Funder> CreateFunder(RefDataBaseDto funder);
        Task<Funder> UpdateFunder(int id, RefDataBaseDto funder);
        Task DeleteFunder(int id);
        Task<HtaStatus> CreateHtaStatus(SortedRefDataBaseDto htaStatus);
        Task<HtaStatus> UpdateHtaStatus(int id, SortedRefDataBaseDto htaStatus);
        Task DeleteHtaStatus(int id);
        Task<MacroscopicAssessment> CreateMacroscopicAssessment(RefDataBaseDto macroscopicAssessment);
        Task<MacroscopicAssessment> UpdateMacroscopicAssessment(int id, RefDataBaseDto macroscopicAssessment);
        Task DeleteMacroscopicAssessment(int id);
        Task<MaterialType> CreateMaterialType(SortedRefDataBaseDto materialType);
        Task<MaterialType> UpdateMaterialType(int id, SortedRefDataBaseDto materialType);
        Task DeleteMaterialType(int id);
        Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm);
        Task<OntologyTerm> UpdateOntologyTerm(OntologyTerm ontologyTerm);
        Task DeleteOntologyTerm(string id);
        Task<ServiceOffering> CreateServiceOffering(SortedRefDataBaseDto serviceOffering);
        Task<ServiceOffering> UpdateServiceOffering(int id, SortedRefDataBaseDto serviceOffering);
        Task DeleteServiceOffering(int id);
        Task<Sex> CreateSex(SortedRefDataBaseDto sex);
        Task<Sex> UpdateSex(int id, SortedRefDataBaseDto sex);
        Task DeleteSex(int id);
        Task<SopStatus> CreateSopStatus(SortedRefDataBaseDto sopStatus);
        Task<SopStatus> UpdateSopStatus(int id, SortedRefDataBaseDto sopStatus);
        Task DeleteSopStatus(int id);
        Task<StorageTemperature> CreateStorageTemperature(SortedRefDataBaseDto storageTemperature);
        Task<StorageTemperature> UpdateStorageTemperature(int id, SortedRefDataBaseDto storageTemperature);
        Task DeleteStorageTemperature(int id);
    }
}
