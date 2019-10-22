using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Common.Data;
using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Directory.Services
{
    
    public class ReferenceDataReadService : IReferenceDataReadService
    {
        private readonly DirectoryContext _context;
        private readonly IMapper _mapper;

        public ReferenceDataReadService(DirectoryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region AccessCondition

        public async Task<ICollection<AccessCondition>> ListAccessConditions()
            => await _context.AccessConditions.ToListAsync();
   
        public async Task<AccessCondition> GetAccessCondition(int id)
            => await _context.AccessConditions.FindAsync(id);

        #endregion

        #region AgeRange
       
        public async Task<ICollection<AgeRange>> ListAgeRanges()
            => await _context.AgeRanges.ToListAsync();
      
        public async Task<AgeRange> GetAgeRange(int id)
            => await _context.AgeRanges.FindAsync(id);

        #endregion

        #region AnnualStatistic

        public async Task<ICollection<AnnualStatisticOutboundDto>> ListAnnualStatistics()
        {
            var entities = await _context.AnnualStatistics.Include(a => a.AnnualStatisticGroup).ToListAsync();
            return _mapper.Map<ICollection<AnnualStatisticOutboundDto>>(entities);
        }
      
        public async Task<AnnualStatisticOutboundDto> GetAnnualStatistic(int id)
        {
            var entity = await _context.AnnualStatistics.FindAsync(id);
            return _mapper.Map<AnnualStatisticOutboundDto>(entity);
        }

        #endregion

        #region AnnualStatisticGroup

        public async Task<ICollection<AnnualStatisticGroup>> ListAnnualStatisticGroups()
            => await _context.AnnualStatisticGroups.ToListAsync();

        public async Task<AnnualStatisticGroup> GetAnnualStatisticGroup(int id)
            => await _context.AnnualStatisticGroups.FindAsync(id);

        #endregion

        #region AssociatedDataProcurementTimeframe

        public async Task<ICollection<AssociatedDataProcurementTimeframe>> ListAssociatedDataProcurementTimeframes()
            => await _context.AssociatedDataProcurementTimeframes.ToListAsync();
     
        public async Task<AssociatedDataProcurementTimeframe> GetAssociatedDataProcurementTimeframe(int id)
            => await _context.AssociatedDataProcurementTimeframes.FindAsync(id);

        #endregion

        #region AssociatedDataTypes
    
        public async Task<ICollection<AssociatedDataType>> ListAssociatedDataTypes()
            => await _context.AssociatedDataTypes.ToListAsync();
     
        public async Task<AssociatedDataType> GetAssociatedDataType(int id)
            => await _context.AssociatedDataTypes.FindAsync(id);

        #endregion

        #region CollectionPercentage
     
        public async Task<ICollection<CollectionPercentage>> ListCollectionPercentages()
            => await _context.CollectionPercentages.ToListAsync();
    
        public async Task<CollectionPercentage> GetCollectionPercentage(int id)
            => await _context.CollectionPercentages.FindAsync(id);

        #endregion

        #region CollectionPoint
      
        public async Task<ICollection<CollectionPoint>> ListCollectionPoints()
            => await _context.CollectionPoints.ToListAsync();
     
        public async Task<CollectionPoint> GetCollectionPoint(int id)
            => await _context.CollectionPoints.FindAsync(id);

        #endregion

        #region CollectionStatus
    
        public async Task<ICollection<CollectionStatus>> ListCollectionStatuses()
            => await _context.CollectionStatuses.ToListAsync();
     
        public async Task<CollectionStatus> GetCollectionStatus(int id)
            => await _context.CollectionStatuses.FindAsync(id);

        #endregion

        #region CollectionType
      
        public async Task<ICollection<CollectionType>> ListCollectionTypes()
            => await _context.CollectionTypes.ToListAsync();
      
        public async Task<CollectionType> GetCollectionType(int id)
        => await _context.CollectionTypes.FindAsync(id);

        #endregion

        #region ConsentRestriction
     
        public async Task<ICollection<ConsentRestriction>> ListConsentRestrictions()
            => await _context.ConsentRestrictions.ToListAsync();
     
        public async Task<ConsentRestriction> GetConsentRestriction(int id)
            => await _context.ConsentRestrictions.FindAsync(id);

        #endregion

        #region Country
      
        public async Task<ICollection<Country>> ListCountries()
            => await _context.Countries.ToListAsync();
        
        public async Task<Country> GetCountry(int id)
            => await _context.Countries.FindAsync(id);

        #endregion

        #region County

        public async Task<ICollection<CountyOutboundDto>> ListCounties()
        {
            var counties = await _context.Counties.ToListAsync();
            return _mapper.Map<List<CountyOutboundDto>>(counties);
        }

        public async Task<CountyOutboundDto> GetCounty(int id)
        {
            var county = await _context.Counties.FindAsync(id);
            return _mapper.Map<CountyOutboundDto>(county);
        }

        #endregion

        #region DonorCount
      
        public async Task<ICollection<DonorCount>> ListDonorCounts()
            => await _context.DonorCounts.ToListAsync();
       
        public async Task<DonorCount> GetDonorCount(int id)
            => await _context.DonorCounts.FindAsync(id);

        #endregion

        #region Funder
        
        public async Task<ICollection<Funder>> ListFunders()
            => await _context.Funders.ToListAsync();
      
        public async Task<Funder> GetFunder(int id)
            => await _context.Funders.FindAsync(id);

        #endregion

        #region HtaStatus
        
        public async Task<ICollection<HtaStatus>> ListHtaStatuses()
            => await _context.HtaStatuses.ToListAsync();
     
        public async Task<HtaStatus> GetHtaStatus(int id)
            => await _context.HtaStatuses.FindAsync(id);

        #endregion

        #region MacroscopicAssessment
        
        public async Task<ICollection<MacroscopicAssessment>> ListMacroscopicAssessments()
            => await _context.MacroscopicAssessments.ToListAsync();
       
        public async Task<MacroscopicAssessment> GetMacroscopicAssessment(int id)
            => await _context.MacroscopicAssessments.FindAsync(id);

        #endregion

        #region MaterialType
        
        public async Task<ICollection<MaterialTypeOutboundDto>> ListMaterialTypes()
        {
            var entities = await _context.MaterialTypes.Include(x => x.MaterialTypeGroupMaterialTypes).ThenInclude(y => y.MaterialTypeGroup).ToListAsync();
            var dtos = _mapper.Map<List<MaterialTypeOutboundDto>>(entities);
            return dtos;
        }
       
        public async Task<MaterialTypeOutboundDto> GetMaterialType(int id)
        {
            var entity = await _context.MaterialTypes.Include(x => x.MaterialTypeGroupMaterialTypes).ThenInclude(y => y.MaterialTypeGroup).SingleOrDefaultAsync(x => x.Id == id);
            var dto = _mapper.Map<MaterialTypeOutboundDto>(entity);
            return dto;
        }

        #endregion

        #region MaterialTypeGroup

        public async Task<ICollection<MaterialTypeGroup>> ListMaterialTypeGroups()
            => await _context.MaterialTypeGroups.ToListAsync();

        public async Task<MaterialTypeGroup> GetMaterialTypeGroup(int id)
            => await _context.MaterialTypeGroups.FindAsync(id);

        #endregion

        #region OntologyTerm

        public async Task<ICollection<OntologyTerm>> ListOntologyTerms()
            => await _context.OntologyTerms.ToListAsync();
        
        public async Task<OntologyTerm> GetOntologyTerm(string id)
            => await _context.OntologyTerms.FindAsync(id);

        #endregion

        #region ServiceOffering
      
        public async Task<ICollection<ServiceOffering>> ListServiceOfferings()
            => await _context.ServiceOfferings.ToListAsync();
     
        public async Task<ServiceOffering> GetServiceOffering(int id)
            => await _context.ServiceOfferings.FindAsync(id);

        #endregion

        #region Sex
        
        public async Task<ICollection<Sex>> ListSexes()
            => await _context.Sexes.ToListAsync();
      
        public async Task<Sex> GetSex(int id)
            => await _context.Sexes.FindAsync(id);

        #endregion

        #region SopStatus
   
        public async Task<ICollection<SopStatus>> ListSopStatuses()
            => await _context.SopStatuses.ToListAsync();
    
        public async Task<SopStatus> GetSopStatus(int id)
            => await _context.SopStatuses.FindAsync(id);

        #endregion

        #region StorageTemperature
     
        public async Task<ICollection<StorageTemperature>> ListStorageTemperatures()
            => await _context.StorageTemperatures.ToListAsync();
      
        public async Task<StorageTemperature> GetStorageTemperature(int id)
            => await _context.StorageTemperatures.FindAsync(id);

        #endregion

    }
}
