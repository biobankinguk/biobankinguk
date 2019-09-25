using Common.Data.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Contracts
{
    public interface IReferenceDataReadService
    {
        /// <summary>
        /// Get all Access Conditions
        /// </summary>
        /// <returns></returns>
        Task<ICollection<AccessCondition>> ListAccessConditions();

        /// <summary>
        /// Get an Access Condition by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AccessCondition> GetAccessCondition(int id);

        /// <summary>
        /// Get all Age Ranges
        /// </summary>
        /// <returns></returns>
        Task<ICollection<AgeRange>> ListAgeRanges();

        /// <summary>
        /// Get an Age Range by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AgeRange> GetAgeRange(int id);

        /// <summary>
        /// Get all Annual Statistics
        /// </summary>
        /// <returns></returns>
        Task<ICollection<AnnualStatistic>> ListAnnualStatistics();

        /// <summary>
        /// Get an Annual Statistic by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AnnualStatistic> GetAnnualStatistic(int id);

        /// <summary>
        /// Get all Associated Data Procurement Timeframes
        /// </summary>
        /// <returns></returns>
        Task<ICollection<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeframes();

        /// <summary>
        /// Get an Associated Date Procurement Timeframe by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AssociatedDataProcurementTimeframe> GetAssociatedDataProcurementTimeframe(int id);

        /// <summary>
        /// Get all Associated Data Types
        /// </summary>
        /// <returns></returns>
        Task<ICollection<AssociatedDataType>> ListAssociatedDateTypes();

        /// <summary>
        /// Get an Associated Data Type by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<AssociatedDataType> GetAssociatedDataType(int id);

        /// <summary>
        /// Get all Collection Percentages
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CollectionPercentage>> ListCollectionPercentages();

        /// <summary>
        /// Get a Collection Percentage by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionPercentage> GetCollectionPercentage(int id);

        /// <summary>
        /// Get all Collection Points
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CollectionPoint>> ListCollectionPoints();

        /// <summary>
        /// Get a Collection Point by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionPoint> GetCollectionPoint(int id);

        /// <summary>
        /// Get all Collection Statuses
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CollectionStatus>> ListCollectionStatuses();

        /// <summary>
        /// Get a Collection Status by its ID
        /// </summary>
        /// <returns></returns>
        Task<CollectionStatus> GetCollectionStatus(int id);

        /// <summary>
        /// Get all Collection Types
        /// </summary>
        /// <returns></returns>
        Task<ICollection<CollectionType>> ListCollectionTypes();

        /// <summary>
        /// Get a Collection Type by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CollectionType> GetCollectionType(int id);

        /// <summary>
        /// Get all Consent Restrictions
        /// </summary>
        /// <returns></returns>
        Task<ICollection<ConsentRestriction>> ListConsentRestrictions();

        /// <summary>
        /// Get a Consent Restriction by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ConsentRestriction> GetConsentRestriction(int id);

        /// <summary>
        /// Get all Countries
        /// </summary>
        /// <returns></returns>
        Task<ICollection<Country>> ListCountries();

        /// <summary>
        /// Get a Country by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Country> GetCountry(int id);

        /// <summary>
        /// Get all Counties
        /// </summary>
        /// <returns></returns>
        Task<ICollection<County>> ListCounties();

        /// <summary>
        /// List a County by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<County> GetCounty(int id);

        /// <summary>
        /// Get all Donor Counts
        /// </summary>
        /// <returns></returns>
        Task<ICollection<DonorCount>> ListDonorCounts();

        /// <summary>
        /// Get a Donor Count by its ID
        /// </summary>
        /// <returns></returns>
        Task<DonorCount> GetDonorCount(int id);

        /// <summary>
        /// Get all Funders
        /// </summary>
        /// <returns></returns>
        Task<ICollection<Funder>> ListFunders();

        /// <summary>
        /// Get a Funder by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Funder> GetFunder(int id);

        /// <summary>
        /// Get all HTA Statuses
        /// </summary>
        /// <returns></returns>
        Task<ICollection<HtaStatus>> ListHtaStatuses();

        /// <summary>
        /// Get a HTA Status by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<HtaStatus> GetHtaStatus(int id);

        /// <summary>
        /// Get all Macroscopic Assessments
        /// </summary>
        /// <returns></returns>
        Task<ICollection<MacroscopicAssessment>> ListMacroscopicAssessments();

        /// <summary>
        /// Get a Macroscopic Assessment by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MacroscopicAssessment> GetMacroscopicAssessment(int id);

        /// <summary>
        /// Get all Material Types
        /// </summary>
        /// <returns></returns>
        Task<ICollection<MaterialType>> ListMaterialTypes();

        /// <summary>
        /// Get a Material Type by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<MaterialType> GetMaterialType(int id);

        /// <summary>
        /// Get all Ontology Terms
        /// </summary>
        /// <returns></returns>
        Task<ICollection<OntologyTerm>> ListOntologyTerms();

        /// <summary>
        /// Get an Ontology Term by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<OntologyTerm> GetOntologyTerm(string id);

        /// <summary>
        /// Get all Service Offerings
        /// </summary>
        /// <returns></returns>
        Task<ICollection<ServiceOffering>> ListServiceOfferings();

        /// <summary>
        /// Get a Service Offering by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ServiceOffering> GetServiceOffering(int id);

        /// <summary>
        /// Get all Sexes
        /// </summary>
        /// <returns></returns>
        Task<ICollection<Sex>> ListSexes();

        /// <summary>
        /// Get a Sex by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Sex> GetSex(int id);

        /// <summary>
        /// Get all SOP Statuses
        /// </summary>
        /// <returns></returns>
        Task<ICollection<SopStatus>> ListSopStatuses();

        /// <summary>
        /// Get a SOP Status by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SopStatus> GetSopStatus(int id);

        /// <summary>
        /// Get all Storage Temperatures
        /// </summary>
        /// <returns></returns>
        Task<ICollection<StorageTemperature>> ListStorageTemperatures();

        /// <summary>
        /// Get a Storage Temperature by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<StorageTemperature> GetStorageTemperature(int id);
    }
}
