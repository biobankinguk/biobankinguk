using AutoMapper;
using Common.Data;
using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Directory.Services
{
    /// <inheritdoc />
    public class ReferenceDataWriterService : IReferenceDataWriterService
    {
        private readonly DirectoryContext _context;
        private readonly IMapper _mapper;

        public ReferenceDataWriterService(DirectoryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Helper Methods

        /// <summary>
        /// Generic create Ref Data helper method. To be used by all ref data which implements BaseReferenceDatum only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refData"></param>
        /// <returns></returns>
        private async Task<T> CreateRefData<T>(T refData) where T : BaseReferenceDatum
        {
            await _context.AddAsync(refData);
            await _context.SaveChangesAsync();
            return refData;
        }

        /// <summary>
        /// Generic update ref Data helper method. To be used by all ref data which implements BaseReferenceDatum only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="refData"></param>
        /// <returns></returns>
        private async Task<T> UpdateRefData<T>(T refData) where T : BaseReferenceDatum
        {
            _context.Update(refData);
            await _context.SaveChangesAsync();
            return refData;
        }

        /// <summary>
        /// Generic delete ref data helper method. To be used by all ref data which implements BaseReferenceDatum only.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<bool> DeleteRefData<T>(int id) where T : BaseReferenceDatum, new()
        {
            try
            {
                _context.Remove(new T { Id = id });
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                //we need to figure out if this exception was thrown because of the id not existing, or due to another error
                if (await _context.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id) != null)
                {
                    //we found some entries, so it must be another error - let this bubble up
                    throw;
                }
                else
                    return false;
            }
        }

        #endregion

        #region AccessCondition

        public async Task<AccessCondition> CreateAccessCondition(SortedRefDataBaseDto accessCondition)
            => await CreateRefData(_mapper.Map<AccessCondition>(accessCondition));

        public async Task<AccessCondition> UpdateAccessCondition(int id, SortedRefDataBaseDto accessCondition)
        {
            var entity = _mapper.Map<AccessCondition>(accessCondition);
            entity.Id = id;
            return await UpdateRefData(entity);
        }


        public async Task<bool> DeleteAccessCondition(int id)
            => await DeleteRefData<AccessCondition>(id);

        #endregion AccessCondition

        #region AgeRange

        public async Task<AgeRange> CreateAgeRange(SortedRefDataBaseDto ageRange)
            => await CreateRefData(_mapper.Map<AgeRange>(ageRange));

        public async Task<AgeRange> UpdateAgeRange(int id, SortedRefDataBaseDto ageRange)
        {
            var entity = _mapper.Map<AgeRange>(ageRange);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteAgeRange(int id)
            => await DeleteRefData<AgeRange>(id);

        #endregion

        #region AnnualStatistic

        public async Task<AnnualStatistic> CreateAnnualStatistic(AnnualStatisticDto annualStatistic)
        {
            var entity = _mapper.Map<AnnualStatistic>(annualStatistic);
            entity.AnnualStatisticGroup = await _context.AnnualStatisticGroups.SingleOrDefaultAsync(x => x.Id == annualStatistic.AnnualStatisticGroupId);
            return await CreateRefData(entity);
        }

        public async Task<AnnualStatistic> UpdateAnnualStatistic(int id, AnnualStatisticDto annualStatistic)
        {
            var entity = _mapper.Map<AnnualStatistic>(annualStatistic);
            entity.Id = id;
            entity.AnnualStatisticGroup = await _context.AnnualStatisticGroups.SingleOrDefaultAsync(x => x.Id == annualStatistic.AnnualStatisticGroupId);
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteAnnualStatistic(int id)
            => await DeleteRefData<AnnualStatistic>(id);

        #endregion

        #region Annual Statistic Groups

        public async Task<AnnualStatisticGroup> CreateAnnualStatisticGroup(SortedRefDataBaseDto annualStatisticGroup)
             => await CreateRefData(_mapper.Map<AnnualStatisticGroup>(annualStatisticGroup));
        

        public async Task<AnnualStatisticGroup> UpdateAnnualStatisticGroup(int id, SortedRefDataBaseDto annualStatisticGroup)
        {
            var entity = _mapper.Map<AnnualStatisticGroup>(annualStatisticGroup);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteAnnualStatisticGroup(int id)
            => await DeleteRefData<AnnualStatisticGroup>(id);

        #endregion

        #region AssociatedDataProcurementTimeframe

        public async Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(SortedRefDataBaseDto associatedDataProcurementTimeframe)
            => await CreateRefData(_mapper.Map<AssociatedDataProcurementTimeframe>(associatedDataProcurementTimeframe));

        public async Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(int id, SortedRefDataBaseDto associatedDataProcurementTimeframe)
        {
            var entity = _mapper.Map<AssociatedDataProcurementTimeframe>(associatedDataProcurementTimeframe);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteAssociatedDataProcurementTimeframe(int id)
        => await DeleteRefData<AssociatedDataProcurementTimeframe>(id);

        #endregion

        #region AssociatedDataType

        public async Task<AssociatedDataType> CreateAssociatedDataType(RefDataBaseDto associatedDataType)
            => await CreateRefData(_mapper.Map<AssociatedDataType>(associatedDataType));

        public async Task<AssociatedDataType> UpdateAssociatedDataType(int id, RefDataBaseDto associatedDataType)
        {
            var entity = _mapper.Map<AssociatedDataType>(associatedDataType);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteAssociatedDataType(int id)
        => await DeleteRefData<AssociatedDataType>(id);

        #endregion

        #region CollectionPercentage

        public async Task<CollectionPercentage> CreateCollectionPercentage(SortedRefDataBaseDto collectionPercentage)
            => await CreateRefData(_mapper.Map<CollectionPercentage>(collectionPercentage));

        public async Task<CollectionPercentage> UpdateCollectionPercentage(int id, SortedRefDataBaseDto collectionPercentage)
        {
            var entity = _mapper.Map<CollectionPercentage>(collectionPercentage);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCollectionPercentage(int id)
            => await DeleteRefData<CollectionPercentage>(id);

        #endregion

        #region CollectionPoint

        public async Task<CollectionPoint> CreateCollectionPoint(SortedRefDataBaseDto collectionPoint)
            => await CreateRefData(_mapper.Map<CollectionPoint>(collectionPoint));

        public async Task<CollectionPoint> UpdateCollectionPoint(int id, SortedRefDataBaseDto collectionPoint)
        {
            var entity = _mapper.Map<CollectionPoint>(collectionPoint);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCollectionPoint(int id)
            => await DeleteRefData<CollectionPoint>(id);

        #endregion

        #region CollectionStatus

        public async Task<CollectionStatus> CreateCollectionStatus(SortedRefDataBaseDto collectionStatus)
            => await CreateRefData(_mapper.Map<CollectionStatus>(collectionStatus));

        public async Task<CollectionStatus> UpdateCollectionStatus(int id, SortedRefDataBaseDto collectionStatus)
        {
            var entity = _mapper.Map<CollectionStatus>(collectionStatus);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCollectionStatus(int id)
            => await DeleteRefData<CollectionStatus>(id);

        #endregion

        #region CollectionType

        public async Task<CollectionType> CreateCollectionType(SortedRefDataBaseDto collectionType)
            => await CreateRefData(_mapper.Map<CollectionType>(collectionType));

        public async Task<CollectionType> UpdateCollectionType(int id, SortedRefDataBaseDto collectionType)
        {
            var entity = _mapper.Map<CollectionType>(collectionType);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCollectionType(int id)
            => await DeleteRefData<CollectionType>(id);

        #endregion

        #region ConsentRestriction

        public async Task<ConsentRestriction> CreateConsentRestriction(SortedRefDataBaseDto consentRestriction)
            => await CreateRefData(_mapper.Map<ConsentRestriction>(consentRestriction));

        public async Task<ConsentRestriction> UpdateConsentRestriction(int id, SortedRefDataBaseDto consentRestriction)
        {
            var entity = _mapper.Map<ConsentRestriction>(consentRestriction);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteConsentRestriction(int id)
            => await DeleteRefData<ConsentRestriction>(id);

        #endregion

        #region Country

        public async Task<Country> CreateCountry(RefDataBaseDto country)
            => await CreateRefData(_mapper.Map<Country>(country));

        public async Task<Country> UpdateCountry(int id, RefDataBaseDto country)
        {
            var entity = _mapper.Map<Country>(country);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCountry(int id)
            => await DeleteRefData<Country>(id);

        #endregion

        #region County

        public async Task<County> CreateCounty(CountyDto county)
        {
            //get a country, assign to County
            var country = await _context.Countries.FindAsync(county.CountryId);
            var entity = _mapper.Map<County>(county);
            entity.Country = country;

            return await CreateRefData(entity);
        }

        public async Task<County> UpdateCounty(int id, CountyDto county)
        {
            //get a country, assign to County
            var country = await _context.Countries.FindAsync(county.CountryId);

            var entity = _mapper.Map<County>(county);
            entity.Id = id;
            entity.Country = country;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteCounty(int id)
            => await DeleteRefData<County>(id);

        #endregion

        #region DonorCount

        public async Task<DonorCount> CreateDonorCount(SortedRefDataBaseDto donorCount)
            => await CreateRefData(_mapper.Map<DonorCount>(donorCount));

        public async Task<DonorCount> UpdateDonorCount(int id, SortedRefDataBaseDto donorCount)
        {
            var entity = _mapper.Map<DonorCount>(donorCount);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteDonorCount(int id)
            => await DeleteRefData<DonorCount>(id);

        #endregion

        #region Funder

        public async Task<Funder> CreateFunder(RefDataBaseDto funder)
            => await CreateRefData(_mapper.Map<Funder>(funder));

        public async Task<Funder> UpdateFunder(int id, RefDataBaseDto funder)
        {
            var entity = _mapper.Map<Funder>(funder);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteFunder(int id)
            => await DeleteRefData<Funder>(id);

        #endregion

        #region HtaStatus

        public async Task<HtaStatus> CreateHtaStatus(SortedRefDataBaseDto htaStatus)
            => await CreateRefData(_mapper.Map<HtaStatus>(htaStatus));

        public async Task<HtaStatus> UpdateHtaStatus(int id, SortedRefDataBaseDto htaStatus)
        {
            var entity = _mapper.Map<HtaStatus>(htaStatus);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteHtaStatus(int id)
            => await DeleteRefData<HtaStatus>(id);

        #endregion

        #region MacroscopicAssessment

        public async Task<MacroscopicAssessment> CreateMacroscopicAssessment(RefDataBaseDto macroscopicAssessment)
            => await CreateRefData(_mapper.Map<MacroscopicAssessment>(macroscopicAssessment));

        public async Task<MacroscopicAssessment> UpdateMacroscopicAssessment(int id, RefDataBaseDto macroscopicAssessment)
        {
            var entity = _mapper.Map<MacroscopicAssessment>(macroscopicAssessment);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteMacroscopicAssessment(int id)
            => await DeleteRefData<MacroscopicAssessment>(id);

        #endregion

        #region MaterialType

        public async Task<(int, MaterialTypeDto)> CreateMaterialType(MaterialTypeDto materialType)
        {
            var entity = _mapper.Map<MaterialType>(materialType);

            foreach (var x in materialType.MaterialTypeGroups)
            {
                    var materialTypeGroup = await _context.MaterialTypeGroups.FindAsync(x.GroupId);

                    var joiningEntity = new MaterialTypeGroupMaterialType { MaterialType = entity, MaterialTypeGroup = materialTypeGroup };

                    entity.MaterialTypeGroupMaterialTypes.Add(joiningEntity );
            }
                await CreateRefData(entity);

                //convert back to DTO
                return (entity.Id, _mapper.Map<MaterialTypeDto>(entity));
        }

        public async Task<MaterialTypeDto> UpdateMaterialType(int id, MaterialTypeDto materialType)
        { 
            var entity = _mapper.Map<MaterialType>(materialType);
            entity.Id = id;

            var existingJoinEntities = _context.MaterialTypeGroupMaterialTypes.Where(x => x.MaterialTypeId == id).ToList();

            foreach (var x in materialType.MaterialTypeGroups)
            {
                if (existingJoinEntities.FindIndex(y => y.MaterialTypeGroupId == x.GroupId) > 0)
                {
                    entity.MaterialTypeGroupMaterialTypes.Add(new MaterialTypeGroupMaterialType
                    { MaterialType = entity, MaterialTypeGroup = await _context.MaterialTypeGroups.FindAsync(x.GroupId) });
                }
            }
            //we now need to check for any Join Entities which have been deleted in the client
            foreach(var x in existingJoinEntities.Except(entity.MaterialTypeGroupMaterialTypes))
            {
                _context.Remove(x);
            }

            await UpdateRefData(entity);

            return _mapper.Map<MaterialTypeDto>(entity);
        }

        public async Task<bool> DeleteMaterialType(int id)
            => await DeleteRefData<MaterialType>(id);

        #endregion

        #region MaterialTypeGroup

        public async Task<MaterialTypeGroup> CreateMaterialTypeGroup(RefDataBaseDto materialTypeGroup)
            => await CreateRefData(_mapper.Map<MaterialTypeGroup>(materialTypeGroup));

        public async Task<MaterialTypeGroup> UpdateMaterialTypeGroup(int id, RefDataBaseDto materialTypeGroup)
        {
            var entity = _mapper.Map<MaterialTypeGroup>(materialTypeGroup);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteMaterialTypeGroup(int id)
            => await DeleteRefData<MaterialTypeGroup>(id);

        #endregion

        #region OntologyTerm

        public async Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm)
        {
            await _context.AddAsync(ontologyTerm);
            await _context.SaveChangesAsync();
            return ontologyTerm;
        }

        public async Task<OntologyTerm> UpdateOntologyTerm(string id, OntologyTerm ontologyTerm)
        {
            _context.Update(ontologyTerm);
            await _context.SaveChangesAsync();
            return ontologyTerm;
        }

        public async Task<bool> DeleteOntologyTerm(string id)
        {
            _context.Remove(new OntologyTerm { Id = id });
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region ServiceOffering

        public async Task<ServiceOffering> CreateServiceOffering(SortedRefDataBaseDto serviceOffering)
            => await CreateRefData(_mapper.Map<ServiceOffering>(serviceOffering));

        public async Task<ServiceOffering> UpdateServiceOffering(int id, SortedRefDataBaseDto serviceOffering)
        {
            var entity = _mapper.Map<ServiceOffering>(serviceOffering);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteServiceOffering(int id)
            => await DeleteRefData<ServiceOffering>(id);

        #endregion

        #region Sex

        public async Task<Sex> CreateSex(SortedRefDataBaseDto sex)
            => await CreateRefData(_mapper.Map<Sex>(sex));

        public async Task<Sex> UpdateSex(int id, SortedRefDataBaseDto sex)
        {
            var entity = _mapper.Map<Sex>(sex);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteSex(int id)
            => await DeleteRefData<Sex>(id);

        #endregion

        #region SopStatus

        public async Task<SopStatus> CreateSopStatus(SortedRefDataBaseDto sopStatus)
        {
            return await CreateRefData(_mapper.Map<SopStatus>(sopStatus));
        }

        public async Task<SopStatus> UpdateSopStatus(int id, SortedRefDataBaseDto sopStatus)
        {
            var entity = _mapper.Map<SopStatus>(sopStatus);
            entity.Id = id;
            return await UpdateRefData(entity);
        }


        public async Task<bool> DeleteSopStatus(int id)
            => await DeleteRefData<SopStatus>(id);

        #endregion

        #region StorageTemperature

        public async Task<StorageTemperature> CreateStorageTemperature(SortedRefDataBaseDto storageTemperature)
            => await CreateRefData(_mapper.Map<StorageTemperature>(storageTemperature));

        public async Task<StorageTemperature> UpdateStorageTemperature(int id, SortedRefDataBaseDto storageTemperature)
        {
            var entity = _mapper.Map<StorageTemperature>(storageTemperature);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task<bool> DeleteStorageTemperature(int id)
            => await DeleteRefData<StorageTemperature>(id);

        #endregion



    }
}

