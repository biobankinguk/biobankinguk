using Common.Data;
using Common.Data.ReferenceData;
using Directory.Contracts;
using System.Threading.Tasks;

namespace Directory.Services
{
    /// <inheritdoc />
    public class ReferenceDataWriterService : IReferenceDataWriterService
    {
        private readonly DirectoryContext _context;

        public ReferenceDataWriterService(DirectoryContext context)
        {
            _context = context;
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
        private async Task DeleteRefData<T>(int id) where T : BaseReferenceDatum, new() 
        {
            _context.Remove(new T { Id = id });
            await _context.SaveChangesAsync();
        }

        #endregion

        #region AccessCondition

        public Task<AccessCondition> CreateAccessCondition(AccessCondition accessCondition)
            => CreateRefData(accessCondition);

        public Task<AccessCondition> UpdateAccessCondition(AccessCondition accessCondition)
            => UpdateRefData(accessCondition);

        public Task DeleteAccessCondition(int id)
            => DeleteRefData<AccessCondition>(id);

        #endregion AccessCondition

        #region AgeRange

        public Task<AgeRange> CreateAgeRange(AgeRange ageRange)
            => CreateRefData(ageRange);

        public Task<AgeRange> UpdateAgeRange(AgeRange ageRange)
            => UpdateRefData(ageRange);

        public Task DeleteAgeRange(int id)
            => DeleteRefData<AgeRange>(id);

        #endregion

        #region AnnualStatistic

        public Task<AnnualStatistic> CreateAnnualStatistic(AnnualStatistic annualStatistic)
            => CreateRefData(annualStatistic);

        public Task<AnnualStatistic> UpdateAnnualStatistic(AnnualStatistic annualStatistic)
            => UpdateRefData(annualStatistic);

        public Task DeleteAnnualStatistic(int id)
            => DeleteRefData<AnnualStatistic>(id);

        #endregion

        #region AssociatedDataProcurementTimeframe

        public Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
            => CreateRefData(associatedDataProcurementTimeframe);

        public Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
            => UpdateRefData(associatedDataProcurementTimeframe);

        public Task DeleteAssociatedDataProcurementTimeframe(int id)
            => DeleteRefData<AssociatedDataProcurementTimeframe>(id);

        #endregion

        #region AssociatedDataType

        public Task<AssociatedDataType> CreateAssociatedDataType(AssociatedDataType associatedDataType)
            => CreateRefData(associatedDataType);

        public Task<AssociatedDataType> UpdateAssociatedDataType(AssociatedDataType associatedDataType)
            => UpdateRefData(associatedDataType);

        public Task DeleteAssociatedDataType(int id)
            => DeleteRefData<AssociatedDataType>(id);

        #endregion

        #region CollectionPercentage

        public Task<CollectionPercentage> CreateCollectionPercentage(CollectionPercentage collectionPercentage)
            => CreateRefData(collectionPercentage);

        public Task<CollectionPercentage> UpdateCollectionPercentage(CollectionPercentage collectionPercentage)
            => UpdateRefData(collectionPercentage);

        public Task DeleteCollectionPercentage(int id)
            => DeleteRefData<CollectionPercentage>(id);

        #endregion

        #region CollectionPoint

        public Task<CollectionPoint> CreateCollectionPoint(CollectionPoint collectionPoint)
            => CreateRefData(collectionPoint);

        public Task<CollectionPoint> UpdateCollectionPoint(CollectionPoint collectionPoint)
            => UpdateRefData(collectionPoint);

        public Task DeleteCollectionPoint(int id)
            => DeleteRefData<CollectionPoint>(id);

        #endregion

        #region CollectionStatus

        public Task<CollectionStatus> CreateCollectionStatus(CollectionStatus collectionStatus)
            => CreateRefData(collectionStatus);

        public Task<CollectionStatus> UpdateCollectionStatus(CollectionStatus collectionStatus)
            => UpdateRefData(collectionStatus);

        public Task DeleteCollectionStatus(int id)
            => DeleteRefData<CollectionStatus>(id);

        #endregion

        #region CollectionType

        public Task<CollectionType> CreateCollectionType(CollectionType collectionType)
            => CreateRefData(collectionType);

        public Task<CollectionType> UpdateCollectionType(CollectionType collectionType)
            => UpdateRefData(collectionType);

        public Task DeleteCollectionType(int id)
            => DeleteRefData<CollectionType>(id);

        #endregion

        #region ConsentRestriction

        public Task<ConsentRestriction> CreateConsentRestriction(ConsentRestriction consentRestriction)
            => CreateRefData(consentRestriction);

        public Task<ConsentRestriction> UpdateConsentRestriction(ConsentRestriction consentRestriction)
            => UpdateRefData(consentRestriction);

        public Task DeleteConsentRestriction(int id)
            => DeleteRefData<ConsentRestriction>(id);

        #endregion

        #region Country

        public Task<Country> CreateCountry(Country country)
            => CreateRefData(country);

        public Task<Country> UpdateCountry(Country country)
            => UpdateRefData(country);

        public Task DeleteCountry(int id)
            => DeleteRefData<Country>(id);

        #endregion

        #region County

        public Task<County> CreateCounty(County county)
            => CreateRefData(county);

        public Task<County> UpdateCounty(County county)
            => UpdateRefData(county);

        public Task DeleteCounty(int id)
            => DeleteRefData<County>(id);

        #endregion

        #region DonorCount

        public Task<DonorCount> CreateDonorCount(DonorCount donorCount)
            => CreateRefData(donorCount);

        public Task<DonorCount> UpdateDonorCount(DonorCount donorCount)
            => UpdateRefData(donorCount);

        public Task DeleteDonorCount(int id)
            => DeleteRefData<DonorCount>(id);

        #endregion

        #region Funder

        public Task<Funder> CreateFunder(Funder funder)
            => CreateRefData(funder);

        public Task<Funder> UpdateFunder(Funder funder)
            => UpdateRefData(funder);

        public Task DeleteFunder(int id)
            => DeleteRefData<Funder>(id);

        #endregion

        #region HtaStatus

        public Task<HtaStatus> CreateHtaStatus(HtaStatus htaStatus)
            => CreateRefData(htaStatus);

        public Task<HtaStatus> UpdateHtaStatus(HtaStatus htaStatus)
            => UpdateRefData(htaStatus);

        public Task DeleteHtaStatus(int id)
            => DeleteRefData<HtaStatus>(id);

        #endregion

        #region MacroscopicAssessment

        public Task<MacroscopicAssessment> CreateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment)
            => CreateRefData(macroscopicAssessment);

        public Task<MacroscopicAssessment> UpdateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment)
            => UpdateRefData(macroscopicAssessment);

        public Task DeleteMacroscopicAssessment(int id)
            => DeleteRefData<MacroscopicAssessment>(id);

        #endregion

        #region MaterialType

        public Task<MaterialType> CreateMaterialType(MaterialType materialType)
            => CreateRefData(materialType);

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

        public Task<ServiceOffering> CreateServiceOffering(ServiceOffering serviceOffering)
            => CreateRefData(serviceOffering);

        public Task<ServiceOffering> UpdateServiceOffering(ServiceOffering serviceOffering)
            => UpdateRefData(serviceOffering);

        public Task DeleteServiceOffering(int id)
            => DeleteRefData<ServiceOffering>(id);

        #endregion

        #region Sex

        public Task<Sex> CreateSex(Sex sex)
            => CreateRefData(sex);

        public Task<Sex> UpdateSex(Sex sex)
            => UpdateRefData(sex);

        public Task DeleteSex(int id)
            => DeleteRefData<Sex>(id);

        #endregion

        #region SopStatus

        public Task<SopStatus> CreateSopStatus(SopStatus sopStatus)
            => CreateRefData(sopStatus);

        public Task<SopStatus> UpdateSopStatus(SopStatus sopStatus)
            => UpdateRefData(sopStatus);

        public Task DeleteSopStatus(int id)
            => DeleteRefData<SopStatus>(id);

        #endregion

        #region StorageTemperature

        public Task<StorageTemperature> CreateStorageTemperature(StorageTemperature storageTemperature)
            => CreateRefData(storageTemperature);

        public Task<StorageTemperature> UpdateStorageTemperature(StorageTemperature storageTemperature)
            => UpdateRefData(storageTemperature);

        public Task DeleteStorageTemperature(int id)
            => DeleteRefData<StorageTemperature>(id);

        #endregion
    }
}

