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
        Task<bool> DeleteAccessCondition(int id);
        Task<AgeRange> CreateAgeRange(SortedRefDataBaseDto ageRange);
        Task<AgeRange> UpdateAgeRange(int id, SortedRefDataBaseDto ageRange);
        Task<bool> DeleteAgeRange(int id);
        Task<AnnualStatistic> CreateAnnualStatistic(AnnualStatisticInboundDto annualStatistic);
        Task<AnnualStatistic> UpdateAnnualStatistic(int id, AnnualStatisticInboundDto annualStatistic);
        Task<bool> DeleteAnnualStatistic(int id);
        Task<AnnualStatisticGroup> CreateAnnualStatisticGroup(SortedRefDataBaseDto annualStatisticGroup);
        Task<AnnualStatisticGroup> UpdateAnnualStatisticGroup(int id, SortedRefDataBaseDto annualStatisticGroup);
        Task<bool> DeleteAnnualStatisticGroup(int id);
        Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(SortedRefDataBaseDto associatedDataProcurementTimeframe);
        Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(int id, SortedRefDataBaseDto associatedDataProcurementTimeframe);
        Task<bool> DeleteAssociatedDataProcurementTimeframe(int id);
        Task<AssociatedDataType> CreateAssociatedDataType(RefDataBaseDto associatedDataType);
        Task<AssociatedDataType> UpdateAssociatedDataType(int id, RefDataBaseDto associatedDataType);
        Task<bool> DeleteAssociatedDataType(int id);
        Task<CollectionPercentage> CreateCollectionPercentage(SortedRefDataBaseDto collectionPercentage);
        Task<CollectionPercentage> UpdateCollectionPercentage(int id, SortedRefDataBaseDto collectionPercentage);
        Task<bool> DeleteCollectionPercentage(int id);
        Task<CollectionPoint> CreateCollectionPoint(SortedRefDataBaseDto collectionPoint);
        Task<CollectionPoint> UpdateCollectionPoint(int id, SortedRefDataBaseDto collectionPoint);
        Task<bool> DeleteCollectionPoint(int id);
        Task<CollectionStatus> CreateCollectionStatus(SortedRefDataBaseDto collectionStatus);
        Task<CollectionStatus> UpdateCollectionStatus(int id, SortedRefDataBaseDto collectionStatus);
        Task<bool> DeleteCollectionStatus(int id);
        Task<CollectionType> CreateCollectionType(SortedRefDataBaseDto collectionType);
        Task<CollectionType> UpdateCollectionType(int id, SortedRefDataBaseDto collectionType);
        Task<bool> DeleteCollectionType(int id);
        Task<ConsentRestriction> CreateConsentRestriction(SortedRefDataBaseDto consentRestriction);
        Task<ConsentRestriction> UpdateConsentRestriction(int id, SortedRefDataBaseDto consentRestriction);
        Task<bool> DeleteConsentRestriction(int id);
        Task<Country> CreateCountry(RefDataBaseDto country);
        Task<Country> UpdateCountry(int id, RefDataBaseDto country);
        Task<bool> DeleteCountry(int id);
        Task<County> CreateCounty(CountyInboundDto county);
        Task<County> UpdateCounty(int id, CountyInboundDto county);
        Task<bool> DeleteCounty(int id);
        Task<DonorCountOutboundDto> CreateDonorCount(DonorCountInboundDto donorCount);
        Task<DonorCountOutboundDto> UpdateDonorCount(int id, DonorCountInboundDto donorCount);
        Task<bool> DeleteDonorCount(int id);
        Task<Funder> CreateFunder(RefDataBaseDto funder);
        Task<Funder> UpdateFunder(int id, RefDataBaseDto funder);
        Task<bool> DeleteFunder(int id);
        Task<HtaStatus> CreateHtaStatus(SortedRefDataBaseDto htaStatus);
        Task<HtaStatus> UpdateHtaStatus(int id, SortedRefDataBaseDto htaStatus);
        Task<bool> DeleteHtaStatus(int id);
        Task<MacroscopicAssessment> CreateMacroscopicAssessment(RefDataBaseDto macroscopicAssessment);
        Task<MacroscopicAssessment> UpdateMacroscopicAssessment(int id, RefDataBaseDto macroscopicAssessment);
        Task<bool> DeleteMacroscopicAssessment(int id);
        Task<MaterialTypeOutboundDto> CreateMaterialType(MaterialTypeInboundDto materialType);
        Task<MaterialTypeOutboundDto> UpdateMaterialType(int id, MaterialTypeInboundDto materialType);
        Task<bool> DeleteMaterialType(int id);
        Task<MaterialTypeGroupOutboundDto> CreateMaterialTypeGroup(MaterialTypeGroupInboundDto materialTypeGroup);
        Task<MaterialTypeGroupOutboundDto> UpdateMaterialTypeGroup(int id, MaterialTypeGroupInboundDto materialTypeGroup);
        Task<bool> DeleteMaterialTypeGroup(int id);
        Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm);
        Task<OntologyTerm> UpdateOntologyTerm(string id, OntologyTerm ontologyTerm);
        Task<bool> DeleteOntologyTerm(string id);
        Task<ServiceOffering> CreateServiceOffering(SortedRefDataBaseDto serviceOffering);
        Task<ServiceOffering> UpdateServiceOffering(int id, SortedRefDataBaseDto serviceOffering);
        Task<bool> DeleteServiceOffering(int id);
        Task<Sex> CreateSex(SortedRefDataBaseDto sex);
        Task<Sex> UpdateSex(int id, SortedRefDataBaseDto sex);
        Task<bool> DeleteSex(int id);
        Task<SopStatus> CreateSopStatus(SortedRefDataBaseDto sopStatus);
        Task<SopStatus> UpdateSopStatus(int id, SortedRefDataBaseDto sopStatus);
        Task<bool> DeleteSopStatus(int id);
        Task<StorageTemperature> CreateStorageTemperature(SortedRefDataBaseDto storageTemperature);
        Task<StorageTemperature> UpdateStorageTemperature(int id, SortedRefDataBaseDto storageTemperature);
        Task<bool> DeleteStorageTemperature(int id);
    }
}
