using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Data;
using Common.Data.ReferenceData;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Directory.Services
{
    /// <inheritdoc />
    public class ReferenceDataReadService : IReferenceDataReadService
    {
        private readonly DirectoryContext _context;

        public ReferenceDataReadService(DirectoryContext context)
        {
            _context = context;
        }

        #region AccessCondition

        /// <inheritdoc />
        public async Task<ICollection<AccessCondition>> ListAccessConditions()
            => await _context.AccessConditions.ToListAsync();

        /// <inheritdoc />
        public async Task<AccessCondition> GetAccessCondition(int id)
            => await _context.AccessConditions.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AgeRange

        /// <inheritdoc />
        public async Task<ICollection<AgeRange>> ListAgeRanges()
            => await _context.AgeRanges.ToListAsync();

        /// <inheritdoc />
        public async Task<AgeRange> GetAgeRange(int id)
            => await _context.AgeRanges.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AnnualStatistic

        /// <inheritdoc />
        public async Task<ICollection<AnnualStatistic>> ListAnnualStatistics()
            => await _context.AnnualStatistics.ToListAsync();

        /// <inheritdoc />
        public async Task<AnnualStatistic> GetAnnualStatistic(int id)
            => await _context.AnnualStatistics.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AssociatedDataProcurementTimeframe

        /// <inheritdoc />
        public async Task<ICollection<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeframes()
            => await _context.AssociatedDataProcurementTimeframes.ToListAsync();

        /// <inheritdoc />
        public async Task<AssociatedDataProcurementTimeframe> GetAssociatedDataProcurementTimeframe(int id)
            => await _context.AssociatedDataProcurementTimeframes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AssociatedDataTypes

        /// <inheritdoc />
        public async Task<ICollection<AssociatedDataType>> ListAssociatedDateTypes()
            => await _context.AssociatedDataTypes.ToListAsync();

        /// <inheritdoc />
        public async Task<AssociatedDataType> GetAssociatedDataType(int id)
            => await _context.AssociatedDataTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionPercentage

        /// <inheritdoc />
        public async Task<ICollection<CollectionPercentage>> ListCollectionPercentages()
            => await _context.CollectionPercentages.ToListAsync();

        /// <inheritdoc />
        public async Task<CollectionPercentage> GetCollectionPercentage(int id)
            => await _context.CollectionPercentages.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionPoint

        /// <inheritdoc />
        public async Task<ICollection<CollectionPoint>> ListCollectionPoints()
            => await _context.CollectionPoints.ToListAsync();

        /// <inheritdoc />
        public async Task<CollectionPoint> GetCollectionPoint(int id)
            => await _context.CollectionPoints.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionStatus

        /// <inheritdoc />
        public async Task<ICollection<CollectionStatus>> ListCollectionStatuses()
            => await _context.CollectionStatuses.ToListAsync();

        /// <inheritdoc />
        public async Task<CollectionStatus> GetCollectionStatus(int id)
            => await _context.CollectionStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionType

        /// <inheritdoc />
        public async Task<ICollection<CollectionType>> ListCollectionTypes()
            => await _context.CollectionTypes.ToListAsync();

        /// <inheritdoc />
        public async Task<CollectionType> GetCollectionType(int id)
        => await _context.CollectionTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region ConsentRestriction

        /// <inheritdoc />
        public async Task<ICollection<ConsentRestriction>> ListConsentRestrictions()
            => await _context.ConsentRestrictions.ToListAsync();

        /// <inheritdoc />
        public async Task<ConsentRestriction> GetConsentRestriction(int id)
            => await _context.ConsentRestrictions.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Country

        /// <inheritdoc />
        public async Task<ICollection<Country>> ListCountries()
            => await _context.Countries.ToListAsync();

        /// <inheritdoc />
        public async Task<Country> GetCountry(int id)
            => await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region County

        /// <inheritdoc />
        public async Task<ICollection<County>> ListCounties()
            => await _context.Counties.ToListAsync();

        /// <inheritdoc />
        public async Task<County> GetCounty(int id)
            => await _context.Counties.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region DonorCount

        /// <inheritdoc />
        public async Task<ICollection<DonorCount>> ListDonorCounts()
            => await _context.DonorCounts.ToListAsync();

        /// <inheritdoc />
        public async Task<DonorCount> GetDonorCount(int id)
            => await _context.DonorCounts.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Funder

        /// <inheritdoc />
        public async Task<ICollection<Funder>> ListFunders()
            => await _context.Funders.ToListAsync();

        /// <inheritdoc />
        public async Task<Funder> GetFunder(int id)
            => await _context.Funders.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region HtaStatus

        /// <inheritdoc />
        public async Task<ICollection<HtaStatus>> ListHtaStatuses()
            => await _context.HtaStatuses.ToListAsync();

        /// <inheritdoc />
        public async Task<HtaStatus> GetHtaStatus(int id)
            => await _context.HtaStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region MacroscopicAssessment

        /// <inheritdoc />
        public async Task<ICollection<MacroscopicAssessment>> ListMacroscopicAssessments()
            => await _context.MacroscopicAssessments.ToListAsync();

        /// <inheritdoc />
        public async Task<MacroscopicAssessment> GetMacroscopicAssessment(int id)
            => await _context.MacroscopicAssessments.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region MaterialType

        /// <inheritdoc />
        public async Task<ICollection<MaterialType>> ListMaterialTypes()
            => await _context.MaterialTypes.ToListAsync();

        /// <inheritdoc />
        public async Task<MaterialType> GetMaterialType(int id)
            => await _context.MaterialTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region OntologyTerm

        /// <inheritdoc />
        public async Task<ICollection<OntologyTerm>> ListOntologyTerms()
            => await _context.OntologyTerms.ToListAsync();

        /// <inheritdoc />
        public async Task<OntologyTerm> GetOntologyTerm(string id)
            => await _context.OntologyTerms.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region ServiceOffering

        /// <inheritdoc />
        public async Task<ICollection<ServiceOffering>> ListServiceOfferings()
            => await _context.ServiceOfferings.ToListAsync();

        /// <inheritdoc />
        public async Task<ServiceOffering> GetServiceOffering(int id)
            => await _context.ServiceOfferings.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Sex

        /// <inheritdoc />
        public async Task<ICollection<Sex>> ListSexes()
            => await _context.Sexes.ToListAsync();

        /// <inheritdoc />
        public async Task<Sex> GetSex(int id)
            => await _context.Sexes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region SopStatus

        /// <inheritdoc />
        public async Task<ICollection<SopStatus>> ListSopStatuses()
            => await _context.SopStatuses.ToListAsync();

        /// <inheritdoc />
        public async Task<SopStatus> GetSopStatus(int id)
            => await _context.SopStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region StorageTemperature

        /// <inheritdoc />
        public async Task<ICollection<StorageTemperature>> ListStorageTemperatures()
            => await _context.StorageTemperatures.ToListAsync();

        /// <inheritdoc />
        public async Task<StorageTemperature> GetStorageTemperature(int id)
            => await _context.StorageTemperatures.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        /// <inheritdoc />

    }
}
