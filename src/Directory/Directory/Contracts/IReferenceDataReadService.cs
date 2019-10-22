using Common.Data.ReferenceData;
using Common.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Contracts
{
    /// <summary>
    /// Service class for getting Reference Data Entities from the DB
    /// </summary>
    public interface IReferenceDataReadService
    {
        Task<ICollection<AccessCondition>> ListAccessConditions();
        Task<AccessCondition> GetAccessCondition(int id);
        Task<ICollection<AgeRange>> ListAgeRanges();
        Task<AgeRange> GetAgeRange(int id);
        Task<ICollection<AnnualStatisticOutboundDto>> ListAnnualStatistics();
        Task<AnnualStatisticOutboundDto> GetAnnualStatistic(int id);
        Task<ICollection<AnnualStatisticGroup>> ListAnnualStatisticGroups();
        Task<AnnualStatisticGroup> GetAnnualStatisticGroup(int id);
        Task<ICollection<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeframes();
        Task<AssociatedDataProcurementTimeframe> GetAssociatedDataProcurementTimeframe(int id);
        Task<ICollection<AssociatedDataType>> ListAssociatedDataTypes();
        Task<AssociatedDataType> GetAssociatedDataType(int id);
        Task<ICollection<CollectionPercentage>> ListCollectionPercentages();
        Task<CollectionPercentage> GetCollectionPercentage(int id);
        Task<ICollection<CollectionPoint>> ListCollectionPoints();
        Task<CollectionPoint> GetCollectionPoint(int id);
        Task<ICollection<CollectionStatus>> ListCollectionStatuses();
        Task<CollectionStatus> GetCollectionStatus(int id);
        Task<ICollection<CollectionType>> ListCollectionTypes();
        Task<CollectionType> GetCollectionType(int id);
        Task<ICollection<ConsentRestriction>> ListConsentRestrictions();
        Task<ConsentRestriction> GetConsentRestriction(int id);
        Task<ICollection<Country>> ListCountries();
        Task<Country> GetCountry(int id);
        Task<ICollection<CountyOutboundDto>> ListCounties();
        Task<CountyOutboundDto> GetCounty(int id);
        Task<ICollection<DonorCount>> ListDonorCounts();
        Task<DonorCount> GetDonorCount(int id);
        Task<ICollection<Funder>> ListFunders();
        Task<Funder> GetFunder(int id);
        Task<ICollection<HtaStatus>> ListHtaStatuses();
        Task<HtaStatus> GetHtaStatus(int id);
        Task<ICollection<MacroscopicAssessment>> ListMacroscopicAssessments();
        Task<MacroscopicAssessment> GetMacroscopicAssessment(int id);
        Task<ICollection<MaterialTypeDto>> ListMaterialTypes();
        Task<MaterialTypeDto> GetMaterialType(int id);
        Task<ICollection<MaterialTypeGroup>> ListMaterialTypeGroups();
        Task<MaterialTypeGroup> GetMaterialTypeGroup(int id);
        Task<ICollection<OntologyTerm>> ListOntologyTerms();
        Task<OntologyTerm> GetOntologyTerm(string id);
        Task<ICollection<ServiceOffering>> ListServiceOfferings();
        Task<ServiceOffering> GetServiceOffering(int id);
        Task<ICollection<Sex>> ListSexes();
        Task<Sex> GetSex(int id);
        Task<ICollection<SopStatus>> ListSopStatuses();
        Task<SopStatus> GetSopStatus(int id);
        Task<ICollection<StorageTemperature>> ListStorageTemperatures();
        Task<StorageTemperature> GetStorageTemperature(int id);
    }
}
