using AutoMapper;
using Common.Data;
using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.EntityFrameworkCore;
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

        public async Task<AgeRange> UpdateAgeRange(int id, AgeRange ageRange)
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

        public async Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(int id, AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
        {
            var entity = _mapper.Map<AssociatedDataProcurementTimeframe>(associatedDataProcurementTimeframe);
            entity.Id = id;
            return await UpdateRefData(entity);
        }

            public async Task DeleteAssociatedDataProcurementTimeframe(int id)
            => await DeleteRefData<AssociatedDataProcurementTimeframe>(id);

        #endregion

        #region AssociatedDataType

        public Task<AssociatedDataType> CreateAssociatedDataType(RefDataBaseDto associatedDataType)
            => CreateRefData(_mapper.Map<AssociatedDataType>(associatedDataType));

        public Task<AssociatedDataType> UpdateAssociatedDataType(AssociatedDataType associatedDataType)
            => UpdateRefData(associatedDataType);

        public Task DeleteAssociatedDataType(int id)
            => DeleteRefData<AssociatedDataType>(id);

        #endregion

        #region CollectionPercentage

        public Task<CollectionPercentage> CreateCollectionPercentage(SortedRefDataBaseDto collectionPercentage)
            => CreateRefData(_mapper.Map<CollectionPercentage>(collectionPercentage));

        public Task<CollectionPercentage> UpdateCollectionPercentage(CollectionPercentage collectionPercentage)
            => UpdateRefData(collectionPercentage);

        public Task DeleteCollectionPercentage(int id)
            => DeleteRefData<CollectionPercentage>(id);

        #endregion

        #region CollectionPoint

        public Task<CollectionPoint> CreateCollectionPoint(SortedRefDataBaseDto collectionPoint)
            => CreateRefData(_mapper.Map<CollectionPoint>(collectionPoint));

        public Task<CollectionPoint> UpdateCollectionPoint(CollectionPoint collectionPoint)
            => UpdateRefData(collectionPoint);

        public Task DeleteCollectionPoint(int id)
            => DeleteRefData<CollectionPoint>(id);

        #endregion

        #region CollectionStatus

        public Task<CollectionStatus> CreateCollectionStatus(SortedRefDataBaseDto collectionStatus)
            => CreateRefData(_mapper.Map<CollectionStatus>(collectionStatus));

        public Task<CollectionStatus> UpdateCollectionStatus(CollectionStatus collectionStatus)
            => UpdateRefData(collectionStatus);

        public Task DeleteCollectionStatus(int id)
            => DeleteRefData<CollectionStatus>(id);

        #endregion

        #region CollectionType

        public Task<CollectionType> CreateCollectionType(SortedRefDataBaseDto collectionType)
            => CreateRefData(_mapper.Map<CollectionType>(collectionType));

        public Task<CollectionType> UpdateCollectionType(CollectionType collectionType)
            => UpdateRefData(collectionType);

        public Task DeleteCollectionType(int id)
            => DeleteRefData<CollectionType>(id);

        #endregion

        #region ConsentRestriction

        public Task<ConsentRestriction> CreateConsentRestriction(SortedRefDataBaseDto consentRestriction)
            => CreateRefData(_mapper.Map<ConsentRestriction>(consentRestriction));

        public Task<ConsentRestriction> UpdateConsentRestriction(ConsentRestriction consentRestriction)
            => UpdateRefData(consentRestriction);

        public Task DeleteConsentRestriction(int id)
            => DeleteRefData<ConsentRestriction>(id);

        #endregion

        #region Country

        public Task<Country> CreateCountry(RefDataBaseDto country)
            => CreateRefData(_mapper.Map<Country>(country));

        public Task<Country> UpdateCountry(Country country)
            => UpdateRefData(country);

        public Task DeleteCountry(int id)
            => DeleteRefData<Country>(id);

        #endregion

        #region County

        public Task<County> CreateCounty(RefDataBaseDto county)
            => CreateRefData(_mapper.Map<County>(county));

        public Task<County> UpdateCounty(County county)
            => UpdateRefData(county);

        public Task DeleteCounty(int id)
            => DeleteRefData<County>(id);

        #endregion

        #region DonorCount

        public Task<DonorCount> CreateDonorCount(SortedRefDataBaseDto donorCount)
            => CreateRefData(_mapper.Map<DonorCount>(donorCount));

        public Task<DonorCount> UpdateDonorCount(DonorCount donorCount)
            => UpdateRefData(donorCount);

        public Task DeleteDonorCount(int id)
            => DeleteRefData<DonorCount>(id);

        #endregion

        #region Funder

        public Task<Funder> CreateFunder(RefDataBaseDto funder)
            => CreateRefData(_mapper.Map<Funder>(funder));

        public Task<Funder> UpdateFunder(Funder funder)
            => UpdateRefData(funder);

        public Task DeleteFunder(int id)
            => DeleteRefData<Funder>(id);

        #endregion

        #region HtaStatus

        public Task<HtaStatus> CreateHtaStatus(SortedRefDataBaseDto htaStatus)
            => CreateRefData(_mapper.Map<HtaStatus>(htaStatus));

        public Task<HtaStatus> UpdateHtaStatus(HtaStatus htaStatus)
            => UpdateRefData(htaStatus);

        public Task DeleteHtaStatus(int id)
            => DeleteRefData<HtaStatus>(id);

        #endregion

        #region MacroscopicAssessment

        public Task<MacroscopicAssessment> CreateMacroscopicAssessment(RefDataBaseDto macroscopicAssessment)
            => CreateRefData(_mapper.Map<MacroscopicAssessment>(macroscopicAssessment));

        public Task<MacroscopicAssessment> UpdateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment)
            => UpdateRefData(macroscopicAssessment);

        public Task DeleteMacroscopicAssessment(int id)
            => DeleteRefData<MacroscopicAssessment>(id);

        #endregion

        #region MaterialType

        public Task<MaterialType> CreateMaterialType(SortedRefDataBaseDto materialType)
            => CreateRefData(_mapper.Map<MaterialType>(materialType));

        public Task<MaterialType> UpdateMaterialType(MaterialType materialType)
            => UpdateRefData(materialType);

        public Task DeleteMaterialType(int id)
            => DeleteRefData<MaterialType>(id);

        #endregion

        #region OntologyTerm

        public async Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm)
        {
            await _context.AddAsync(ontologyTerm);
            await _context.SaveChangesAsync();
            return ontologyTerm;
        }

        public async Task<OntologyTerm> UpdateOntologyTerm(OntologyTerm ontologyTerm)
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

        public Task<ServiceOffering> CreateServiceOffering(SortedRefDataBaseDto serviceOffering)
            => CreateRefData(_mapper.Map<ServiceOffering>(serviceOffering));

        public Task<ServiceOffering> UpdateServiceOffering(ServiceOffering serviceOffering)
            => UpdateRefData(serviceOffering);

        public Task DeleteServiceOffering(int id)
            => DeleteRefData<ServiceOffering>(id);

        #endregion

        #region Sex

        public Task<Sex> CreateSex(SortedRefDataBaseDto sex)
            => CreateRefData(_mapper.Map<Sex>(sex));

        public Task<Sex> UpdateSex(Sex sex)
            => UpdateRefData(sex);

        public Task DeleteSex(int id)
            => DeleteRefData<Sex>(id);

        #endregion

        #region SopStatus

        public Task<SopStatus> CreateSopStatus(SortedRefDataBaseDto sopStatus)
            => CreateRefData(_mapper.Map<SopStatus>(sopStatus));

        public Task<SopStatus> UpdateSopStatus(SopStatus sopStatus)
            => UpdateRefData(sopStatus);

        public Task DeleteSopStatus(int id)
            => DeleteRefData<SopStatus>(id);

        #endregion

        #region StorageTemperature

        public Task<StorageTemperature> CreateStorageTemperature(SortedRefDataBaseDto storageTemperature)
            => CreateRefData(_mapper.Map<StorageTemperature>(storageTemperature));

        public Task<StorageTemperature> UpdateStorageTemperature(StorageTemperature storageTemperature)
            => UpdateRefData(storageTemperature);

        public Task DeleteStorageTemperature(int id)
            => DeleteRefData<StorageTemperature>(id);

        #endregion
    }
}

