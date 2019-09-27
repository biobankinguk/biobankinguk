using Common.Data.ReferenceData;
using System.Threading.Tasks;

namespace Directory.Contracts
{
    /// <summary>
    /// Service class for writing, editing and deleting Reference Data entities to/from the DB.
    /// </summary>
    public interface IReferenceDataWriterService
    {
        Task<AccessCondition> CreateAccessCondition(AccessCondition accessCondition);
        Task<AccessCondition> UpdateAccessCondition(AccessCondition accessCondition);
        Task DeleteAccessCondition(int id);
        Task<AgeRange> CreateAgeRange(AgeRange ageRange);
        Task<AgeRange> UpdateAgeRange(AgeRange ageRange);
        Task DeleteAgeRange(int id);
        Task<AnnualStatistic> CreateAnnualStatistic(AnnualStatistic annualStatistic);
        Task<AnnualStatistic> UpdateAnnualStatistic(AnnualStatistic annualStatistic);
        Task DeleteAnnualStatistic(int id);
        Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe);
        Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe);
        Task DeleteAssociatedDataProcurementTimeframe(int id);
        Task<AssociatedDataType> CreateAssociatedDataType(AssociatedDataType associatedDataType);
        Task<AssociatedDataType> UpdateAssociatedDataType(AssociatedDataType associatedDataType);
        Task DeleteAssociatedDataType(int id);
        Task<CollectionPercentage> CreateCollectionPercentage(CollectionPercentage collectionPercentage);
        Task<CollectionPercentage> UpdateCollectionPercentage(CollectionPercentage collectionPercentage);
        Task DeleteCollectionPercentage(int id);
        Task<CollectionPoint> CreateCollectionPoint(CollectionPoint collectionPoint);
        Task<CollectionPoint> UpdateCollectionPoint(CollectionPoint collectionPoint);
        Task DeleteCollectionPoint(int id);
        Task<CollectionStatus> CreateCollectionStatus(CollectionStatus collectionStatus);
        Task<CollectionStatus> UpdateCollectionStatus(CollectionStatus collectionStatus);
        Task DeleteCollectionStatus(int id);
        Task<CollectionType> CreateCollectionType(CollectionType collectionType);
        Task<CollectionType> UpdateCollectionType(CollectionType collectionType);
        Task DeleteCollectionType(int id);
        Task<ConsentRestriction> CreateConsentRestriction(ConsentRestriction consentRestriction);
        Task<ConsentRestriction> UpdateConsentRestriction(ConsentRestriction consentRestriction);
        Task DeleteConsentRestriction(int id);
        Task<Country> CreateCountry(Country country);
        Task<Country> UpdateCountry(Country country);
        Task DeleteCountry(int id);
        Task<County> CreateCounty(County county);
        Task<County> UpdateCounty(County county);
        Task DeleteCounty(int id);
        Task<DonorCount> CreateDonorCount(DonorCount donorCount);
        Task<DonorCount> UpdateDonorCount(DonorCount donorCount);
        Task DeleteDonorCount(int id);
        Task<Funder> CreateFunder(Funder funder);
        Task<Funder> UpdateFunder(Funder funder);
        Task DeleteFunder(int id);
        Task<HtaStatus> CreateHtaStatus(HtaStatus htaStatus);
        Task<HtaStatus> UpdateHtaStatus(HtaStatus htaStatus);
        Task DeleteHtaStatus(int id);
        Task<MacroscopicAssessment> CreateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment);
        Task<MacroscopicAssessment> UpdateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment);
        Task DeleteMacroscopicAssessment(int id);
        Task<MaterialType> CreateMaterialType(MaterialType materialType);
        Task<MaterialType> UpdateMaterialType(MaterialType materialType);
        Task DeleteMaterialType(int id);
        Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm);
        Task<OntologyTerm> UpdateOntologyTerm(OntologyTerm ontologyTerm);
        Task DeleteOntologyTerm(string id);
        Task<ServiceOffering> CreateServiceOffering(ServiceOffering serviceOffering);
        Task<ServiceOffering> UpdateServiceOffering(ServiceOffering serviceOffering);
        Task DeleteServiceOffering(int id);
        Task<Sex> CreateSex(Sex sex);
        Task<Sex> UpdateSex(Sex sex);
        Task DeleteSex(int id);
        Task<SopStatus> CreateSopStatus(SopStatus sopStatus);
        Task<SopStatus> UpdateSopStatus(SopStatus sopStatus);
        Task DeleteSopStatus(int id);
        Task<StorageTemperature> CreateStorageTemperature(StorageTemperature storageTemperature);
        Task<StorageTemperature> UpdateStorageTemperature(StorageTemperature storageTemperature);
        Task DeleteStorageTemperature(int id);
    }
}
