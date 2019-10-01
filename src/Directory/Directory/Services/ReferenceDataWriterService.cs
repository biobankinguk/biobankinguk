using AutoMapper;
using Common.Data;
using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
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
            _context.Entry(refData).State = EntityState.Modified;
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
        private async Task DeleteRefData<T>(int id) where T : BaseReferenceDatum, new() 
        {
            _context.Remove(new T { Id = id });
            await _context.SaveChangesAsync();
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
            

        public async Task DeleteAccessCondition(int id)
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

        public async Task DeleteAgeRange(int id)
            => await DeleteRefData<AgeRange>(id);

        #endregion

        #region AnnualStatistic

        public async Task<AnnualStatistic> CreateAnnualStatistic(RefDataBaseDto annualStatistic)
            => await CreateRefData(_mapper.Map<AnnualStatistic>(annualStatistic));

        public async Task<AnnualStatistic> UpdateAnnualStatistic(int id, RefDataBaseDto annualStatistic)
        {
            var entity = _mapper.Map<AnnualStatistic>(annualStatistic);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task DeleteAnnualStatistic(int id)
            => await DeleteRefData<AnnualStatistic>(id);

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

            public async Task DeleteAssociatedDataProcurementTimeframe(int id)
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

            public async Task DeleteAssociatedDataType(int id)
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

        public async Task DeleteCollectionPercentage(int id)
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

        public async Task DeleteCollectionPoint(int id)
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

        public async  Task DeleteCollectionStatus(int id)
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

        public async Task DeleteCollectionType(int id)
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

        public async Task DeleteConsentRestriction(int id)
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

        public async Task DeleteCountry(int id)
            => await DeleteRefData<Country>(id);

        #endregion

        #region County

        public async Task<County> CreateCounty(RefDataBaseDto county)
            => await CreateRefData(_mapper.Map<County>(county));

        public async Task<County> UpdateCounty(int id, RefDataBaseDto county)
        {
            var entity = _mapper.Map<County>(county);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task DeleteCounty(int id)
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

        public async Task DeleteDonorCount(int id)
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

        public async Task DeleteFunder(int id)
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

        public async Task DeleteHtaStatus(int id)
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

        public async Task DeleteMacroscopicAssessment(int id)
            => await DeleteRefData<MacroscopicAssessment>(id);

        #endregion

        #region MaterialType

        public async Task<MaterialType> CreateMaterialType(SortedRefDataBaseDto materialType)
            => await CreateRefData(_mapper.Map<MaterialType>(materialType));

        public async Task<MaterialType> UpdateMaterialType(int id, SortedRefDataBaseDto materialType)
        {
            var entity = _mapper.Map<MaterialType>(materialType);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

        public async Task DeleteMaterialType(int id)
            => await DeleteRefData<MaterialType>(id);

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

        public async Task DeleteOntologyTerm(string id)
        {
            _context.Remove(new OntologyTerm { Id = id });
            await _context.SaveChangesAsync();
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

        public async Task DeleteServiceOffering(int id)
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

        public async Task DeleteSex(int id)
            => await DeleteRefData<Sex>(id);

        #endregion

        #region SopStatus

        public async Task<SopStatus> CreateSopStatus(SortedRefDataBaseDto sopStatus)
        {
            try
            {
                return await CreateRefData(_mapper.Map<SopStatus>(sopStatus));
            }
            catch(Exception e)
            {
                throw e;

            }
            return null;
         }

        public async Task<SopStatus> UpdateSopStatus(int id, SortedRefDataBaseDto sopStatus)
        {
            var entity = _mapper.Map<SopStatus>(sopStatus);
            entity.Id = id;
            return await UpdateRefData(entity);
        }


        public async Task DeleteSopStatus(int id)
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

        public async Task DeleteStorageTemperature(int id)
            => await DeleteRefData<StorageTemperature>(id);

        #endregion
    }
}

