using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Data;
using Common.Data.ReferenceData;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Directory.Services
{
    
    public class ReferenceDataReadService : IReferenceDataReadService
    {
        private readonly DirectoryContext _context;

        public ReferenceDataReadService(DirectoryContext context)
        {
            _context = context;
        }

        #region AccessCondition

        public async Task<ICollection<AccessCondition>> ListAccessConditions()
            => await _context.AccessConditions.ToListAsync();
   
        public async Task<AccessCondition> GetAccessCondition(int id)
            => await _context.AccessConditions.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AgeRange
       
        public async Task<ICollection<AgeRange>> ListAgeRanges()
            => await _context.AgeRanges.ToListAsync();
      
        public async Task<AgeRange> GetAgeRange(int id)
            => await _context.AgeRanges.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AnnualStatistic
       
        public async Task<ICollection<AnnualStatistic>> ListAnnualStatistics()
            => await _context.AnnualStatistics.ToListAsync();
      
        public async Task<AnnualStatistic> GetAnnualStatistic(int id)
            => await _context.AnnualStatistics.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AssociatedDataProcurementTimeframe
      
        public async Task<ICollection<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeframes()
            => await _context.AssociatedDataProcurementTimeframes.ToListAsync();
     
        public async Task<AssociatedDataProcurementTimeframe> GetAssociatedDataProcurementTimeframe(int id)
            => await _context.AssociatedDataProcurementTimeframes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region AssociatedDataTypes
    
        public async Task<ICollection<AssociatedDataType>> ListAssociatedDateTypes()
            => await _context.AssociatedDataTypes.ToListAsync();
     
        public async Task<AssociatedDataType> GetAssociatedDataType(int id)
            => await _context.AssociatedDataTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionPercentage
     
        public async Task<ICollection<CollectionPercentage>> ListCollectionPercentages()
            => await _context.CollectionPercentages.ToListAsync();
    
        public async Task<CollectionPercentage> GetCollectionPercentage(int id)
            => await _context.CollectionPercentages.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionPoint
      
        public async Task<ICollection<CollectionPoint>> ListCollectionPoints()
            => await _context.CollectionPoints.ToListAsync();
     
        public async Task<CollectionPoint> GetCollectionPoint(int id)
            => await _context.CollectionPoints.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionStatus
    
        public async Task<ICollection<CollectionStatus>> ListCollectionStatuses()
            => await _context.CollectionStatuses.ToListAsync();
     
        public async Task<CollectionStatus> GetCollectionStatus(int id)
            => await _context.CollectionStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region CollectionType
      
        public async Task<ICollection<CollectionType>> ListCollectionTypes()
            => await _context.CollectionTypes.ToListAsync();
      
        public async Task<CollectionType> GetCollectionType(int id)
        => await _context.CollectionTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region ConsentRestriction
     
        public async Task<ICollection<ConsentRestriction>> ListConsentRestrictions()
            => await _context.ConsentRestrictions.ToListAsync();
     
        public async Task<ConsentRestriction> GetConsentRestriction(int id)
            => await _context.ConsentRestrictions.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Country
      
        public async Task<ICollection<Country>> ListCountries()
            => await _context.Countries.ToListAsync();
        
        public async Task<Country> GetCountry(int id)
            => await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region County

        //TODO replace with mapping to DTO  
        public async Task<ICollection<County>> ListCounties()
            => await _context.Counties.ToListAsync();
     
        public async Task<County> GetCounty(int id)
            => await _context.Counties.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region DonorCount
      
        public async Task<ICollection<DonorCount>> ListDonorCounts()
            => await _context.DonorCounts.ToListAsync();
       
        public async Task<DonorCount> GetDonorCount(int id)
            => await _context.DonorCounts.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Funder
        
        public async Task<ICollection<Funder>> ListFunders()
            => await _context.Funders.ToListAsync();
      
        public async Task<Funder> GetFunder(int id)
            => await _context.Funders.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region HtaStatus
        
        public async Task<ICollection<HtaStatus>> ListHtaStatuses()
            => await _context.HtaStatuses.ToListAsync();
     
        public async Task<HtaStatus> GetHtaStatus(int id)
            => await _context.HtaStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region MacroscopicAssessment
        
        public async Task<ICollection<MacroscopicAssessment>> ListMacroscopicAssessments()
            => await _context.MacroscopicAssessments.ToListAsync();
       
        public async Task<MacroscopicAssessment> GetMacroscopicAssessment(int id)
            => await _context.MacroscopicAssessments.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region MaterialType
        
        public async Task<ICollection<MaterialType>> ListMaterialTypes()
            => await _context.MaterialTypes.ToListAsync();
       
        public async Task<MaterialType> GetMaterialType(int id)
            => await _context.MaterialTypes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region OntologyTerm

        public async Task<ICollection<OntologyTerm>> ListOntologyTerms()
            => await _context.OntologyTerms.ToListAsync();
        
        public async Task<OntologyTerm> GetOntologyTerm(string id)
            => await _context.OntologyTerms.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region ServiceOffering
      
        public async Task<ICollection<ServiceOffering>> ListServiceOfferings()
            => await _context.ServiceOfferings.ToListAsync();
     
        public async Task<ServiceOffering> GetServiceOffering(int id)
            => await _context.ServiceOfferings.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region Sex
        
        public async Task<ICollection<Sex>> ListSexes()
            => await _context.Sexes.ToListAsync();
      
        public async Task<Sex> GetSex(int id)
            => await _context.Sexes.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region SopStatus
   
        public async Task<ICollection<SopStatus>> ListSopStatuses()
            => await _context.SopStatuses.ToListAsync();
    
        public async Task<SopStatus> GetSopStatus(int id)
            => await _context.SopStatuses.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        #region StorageTemperature
     
        public async Task<ICollection<StorageTemperature>> ListStorageTemperatures()
            => await _context.StorageTemperatures.ToListAsync();
      
        public async Task<StorageTemperature> GetStorageTemperature(int id)
            => await _context.StorageTemperatures.FirstOrDefaultAsync(x => x.Id == id);

        #endregion

        

    }
}
