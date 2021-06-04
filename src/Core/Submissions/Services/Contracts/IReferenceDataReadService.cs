using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;

namespace Core.Submissions.Services.Contracts
{
    public interface IReferenceDataReadService
    {
        Task<IEnumerable<MaterialType>> ListMaterialTypes();

        Task<IEnumerable<Ontology>> ListOntologies();

        Task<IEnumerable<SampleContentMethod>> ListSampleContentMethods();

        Task<IEnumerable<Sex>> ListSexes();

        //Snomed by Tag
        Task<IEnumerable<SnomedTag>> ListSnomedTags();

        Task<IEnumerable<OntologyTerm>> ListOntologyTerms();

        Task<IEnumerable<StorageTemperature>> ListStorageTemperatures();

        Task<IEnumerable<PreservationType>> ListPreservationTypes();

        Task<IEnumerable<TreatmentLocation>> ListTreatmentLocations();

        Task<OntologyTerm> GetSnomed(string value, string field);

        Task<OntologyTerm> GetSnomedDiagnosis(string value, string field);

        Task<OntologyTerm> GetSnomedTreatment(string value, string field);

        Task<OntologyTerm> GetSnomedBodyOrgan(string value, string field);

        Task<OntologyTerm> GetSnomedExtractionProcedure(string value, string field);

        Task<MaterialType> GetMaterialType(string value);

        Task<SampleContentMethod> GetSampleContentMethod(string value);

        Task<StorageTemperature> GetStorageTemperature(string value);

        Task<PreservationType> GetPreservationType(string value);

        Task<TreatmentLocation> GetTreatmentLocation(string value);

        Task<Ontology> GetOntology(string value);

        Task<Sex> GetSex(string value);
    }
}
