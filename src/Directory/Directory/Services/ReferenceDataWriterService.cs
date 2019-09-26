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
        /// Generic create Ref Data helper method.
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
        /// Generic update ref Data helper method
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
        /// Generic delete ref data helper method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task DeleteRefData<T>(int id) where T : BaseReferenceDatum, new() 
        {
            _context.Remove(new T { Id = id });
            await _context.SaveChangesAsync();
        }

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
        public Task<AnnualStatistic> CreateAnnualStatistic(AnnualStatistic annualStatistic)
            => CreateRefData(annualStatistic);

        public Task<AnnualStatistic> UpdateAnnualStatistic(AnnualStatistic annualStatistic)
            => UpdateRefData(annualStatistic);

        public Task DeleteAnnualStatistic(int id)
            => DeleteRefData<AnnualStatistic>(id);

        public Task<AssociatedDataProcurementTimeframe> CreateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssociatedDataProcurementTimeframe> UpdateAssociatedDataProcurementTimeframe(AssociatedDataProcurementTimeframe associatedDataProcurementTimeframe)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAssociatedDataProcurementTimeframe(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssociatedDataType> CreateAssociatedDataType(AssociatedDataType associatedDataType)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssociatedDataType> UpdateAssociatedDataType(AssociatedDataType associatedDataType)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAssociatedDataType(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionPercentage> CreateCollectionPercentage(CollectionPercentage collectionPercentage)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionPercentage> UpdateCollectionPercentage(CollectionPercentage collectionPercentage)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCollectionPercentage(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionPoint> CreateCollectionPoint(CollectionPoint collectionPoint)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionPoint> UpdateCollectionPoint(CollectionPoint collectionPoint)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCollectionPoint(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionStatus> CreateCollectionStatus(CollectionStatus collectionStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionStatus> UpdateCollectionStatus(CollectionStatus collectionStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCollectionStatus(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionType> CreateCollectionType(CollectionType collectionType)
        {
            throw new System.NotImplementedException();
        }

        public Task<CollectionType> UpdateCollectionType(CollectionType collectionType)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCollectionType(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConsentRestriction> CreateConsentRestriction(ConsentRestriction consentRestriction)
        {
            throw new System.NotImplementedException();
        }

        public Task<ConsentRestriction> UpdateConsentRestriction(ConsentRestriction consentRestriction)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteConsentRestriction(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Country> CreateCountry(Country country)
        {
            throw new System.NotImplementedException();
        }

        public Task<Country> UpdateCountry(Country country)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCountry(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<County> CreateCounty(County county)
        {
            throw new System.NotImplementedException();
        }

        public Task<County> UpdateCounty(County county)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCounty(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<DonorCount> CreateDonorCount(DonorCount donorCount)
        {
            throw new System.NotImplementedException();
        }

        public Task<DonorCount> UpdateDonorCount(DonorCount donorCount)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteDonorCount(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Funder> CreateFunder(Funder funder)
        {
            throw new System.NotImplementedException();
        }

        public Task<Funder> UpdateFunder(Funder funder)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteFunder(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<HtaStatus> CreateHtaStatus(HtaStatus htaStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task<HtaStatus> UpdateHtaStatus(HtaStatus htaStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteHtaStatus(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<MacroscopicAssessment> CreateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment)
        {
            throw new System.NotImplementedException();
        }

        public Task<MacroscopicAssessment> UpdateMacroscopicAssessment(MacroscopicAssessment macroscopicAssessment)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteMacroscopicAssessment(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<MaterialType> CreateMaterialType(MaterialType materialType)
        {
            throw new System.NotImplementedException();
        }

        public Task<MaterialType> UpdateMaterialType(MaterialType materialType)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteMaterialType(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<OntologyTerm> CreateOntologyTerm(OntologyTerm ontologyTerm)
        {
            throw new System.NotImplementedException();
        }

        public Task<OntologyTerm> UpdateOntologyTerm(OntologyTerm ontologyTerm)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteOntologyTerm(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceOffering> CreateServiceOffering(ServiceOffering serviceOffering)
        {
            throw new System.NotImplementedException();
        }

        public Task<ServiceOffering> UpdateServiceOffering(ServiceOffering serviceOffering)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteServiceOffering(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<Sex> CreateSex(Sex sex)
        {
            throw new System.NotImplementedException();
        }

        public Task<Sex> UpdateSex(Sex sex)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteSex(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<SopStatus> CreateSopStatus(SopStatus sopStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task<SopStatus> UpdateSopStatus(SopStatus sopStatus)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteSopStatus(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<StorageTemperature> CreateStorageTemperature(StorageTemperature storageTemperature)
        {
            throw new System.NotImplementedException();
        }

        public Task<StorageTemperature> UpdateStorageTemperature(StorageTemperature storageTemperature)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteStorageTemperature(int id)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region AccessConditions



        #endregion
    }
}
