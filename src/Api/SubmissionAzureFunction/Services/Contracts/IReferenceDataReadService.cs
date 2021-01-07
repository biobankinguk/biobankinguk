using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;
using MaterialType = LegacyData.Entities.MaterialType;

namespace Biobanks.SubmissionAzureFunction.Services.Contracts
{
    public interface IReferenceDataReadService
    {
        Task<IEnumerable<MaterialType>> ListMaterialTypes();

        Task<IEnumerable<Ontology>> ListOntologies();

        Task<IEnumerable<SampleContentMethod>> ListSampleContentMethods();

        Task<IEnumerable<Sex>> ListSexes();

        //Snomed by Tag
        Task<IEnumerable<SnomedTag>> ListSnomedTags();

        Task<IEnumerable<SnomedTerm>> ListSnomedTerms();

        Task<IEnumerable<StorageTemperature>> ListStorageTemperatures();

        Task<IEnumerable<TreatmentLocation>> ListTreatmentLocations();

        Task<SnomedTerm> GetSnomed(string value, string field);

        Task<SnomedTerm> GetSnomedDiagnosis(string value, string field);

        Task<SnomedTerm> GetSnomedTreatment(string value, string field);

        Task<SnomedTerm> GetSnomedBodyOrgan(string value, string field);

        Task<SnomedTerm> GetSnomedExtractionProcedure(string value, string field);

        Task<MaterialType> GetMaterialTypeWithGroups(string value);

        Task<SampleContentMethod> GetSampleContentMethod(string value);

        Task<StorageTemperature> GetStorageTemperature(string value);

        Task<TreatmentLocation> GetTreatmentLocation(string value);

        Task<Ontology> GetOntology(string value);

        Task<Sex> GetSex(string value);
    }
}
