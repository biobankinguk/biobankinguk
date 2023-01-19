using Biobanks.Data;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Submissions.Services
{
    public class ReferenceDataReadService : IReferenceDataReadService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _db;

        private static readonly TimeSpan CacheExpiry = TimeSpan.FromHours(1);

        public ReferenceDataReadService(
            IMemoryCache cache, ApplicationDbContext db)
        {
            _cache = cache;
            _db = db;
        }

        private async Task<T> CacheGetOrCreateWithAbsoluteExpiry<T>(string key, Func<Task<T>> factory)
            => await _cache.GetOrCreateAsync(
                key,
                async i =>
                {
                    i.AbsoluteExpirationRelativeToNow = CacheExpiry;
                    return await factory();
                });

        public async Task<IEnumerable<SnomedTag>> ListSnomedTags()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.SnomedTags,
                async () => await _db.SnomedTags.AsNoTracking()
                    .ToListAsync());

        private async Task<IEnumerable<SnomedTag>> GetSnomedTagsByValues(IReadOnlyCollection<string> values)
            => (await ListSnomedTags())
                .Where(x => values.Contains(x.Value));

        public async Task<IEnumerable<OntologyTerm>> ListOntologyTerms()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.OntologyTerms,
                async () => await _db.OntologyTerms.AsNoTracking()
                    .Include(x => x.SnomedTag)
                    .Include(x => x.MaterialTypes)
                    .ToListAsync());

        public async Task<IEnumerable<Ontology>> ListOntologies()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.Ontologies,
                async () => await _db.Ontologies.AsNoTracking()
                    .Include(x => x.OntologyVersions)
                    .ToListAsync());

        private async Task<OntologyTerm> SnomedLookup(
            string searchQuery, string field, IReadOnlyCollection<string> tags = null)
        {
            //add the tag filter if necessary
            IEnumerable<SnomedTag> snomedTags = null;
            if (tags != null && tags.Count > 0)
            {
                snomedTags = await GetSnomedTagsByValues(tags);
                if (snomedTags == null) throw new Exception($"Could not find the requested SnomedTags: {tags}");
            }

            var allTerms = await ListOntologyTerms();

            if (snomedTags == null || !snomedTags.Any()) return
                    nameof(OntologyField.Description).Equals(field, StringComparison.CurrentCultureIgnoreCase) ?
                    allTerms.FirstOrDefault(t => t.Value == searchQuery) :
                    allTerms.FirstOrDefault(t => t.Id == searchQuery);
            allTerms.FirstOrDefault(t => t.Id == searchQuery);

            var snomedTagIds = snomedTags.Select(st => st.Id);
            var termsForTag = allTerms.Where(t => t.SnomedTag != null && snomedTagIds.Contains(t.SnomedTag.Id));

            return
                nameof(OntologyField.Description).Equals(field, StringComparison.CurrentCultureIgnoreCase) ?
                termsForTag.FirstOrDefault(t => t.Value == searchQuery) :
                termsForTag.FirstOrDefault(t => t.Id == searchQuery);
        }

        public async Task<OntologyTerm> GetSnomed(string value, string field)
            => await SnomedLookup(value, field);

        public async Task<OntologyTerm> GetSnomedDiagnosis(string value, string field)
            => await SnomedLookup(value, field, new[] { "Disease" });

        public async Task<OntologyTerm> GetSnomedTreatment(string value, string field)
            => await SnomedLookup(value, field, new[] { "Treatment" });

        public async Task<OntologyTerm> GetSnomedBodyOrgan(string value, string field)
            => await SnomedLookup(value, field, new[] { "Body Organ" });

        public async Task<OntologyTerm> GetSnomedExtractionProcedure(string value, string field)
            => await SnomedLookup(value, field, new[] { "Extraction Procedure" });

        public async Task<IEnumerable<MaterialType>> ListMaterialTypes()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.MaterialTypes,
                async () => await _db.MaterialTypes.AsNoTracking()
                    .Include(x => x.ExtractionProcedures)
                    .Include(x => x.MaterialTypeGroups)
                    .ToListAsync());

        public async Task<MaterialType> GetMaterialType(string value)
            => (await ListMaterialTypes())
                .FirstOrDefault(x =>
                    x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<SampleContentMethod>> ListSampleContentMethods()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.SampleContentMethods,
                async () => await _db.SampleContentMethods.AsNoTracking()
                    .ToListAsync());

        public async Task<SampleContentMethod> GetSampleContentMethod(string value)
            => (await ListSampleContentMethods())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<StorageTemperature>> ListStorageTemperatures()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.StorageTemperatures,
                async () => await _db.StorageTemperatures.AsNoTracking()
                    .ToListAsync());

        public async Task<StorageTemperature> GetStorageTemperature(string value)
            => (await ListStorageTemperatures())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<PreservationType>> ListPreservationTypes()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.PreservationTypes,
                async () => await _db.PreservationTypes.AsNoTracking()
                    .ToListAsync());

        public async Task<PreservationType> GetPreservationType(string value)
            => (await ListPreservationTypes())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<TreatmentLocation>> ListTreatmentLocations()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.TreatmentLocations,
                async () => await _db.TreatmentLocations.AsNoTracking()
                    .ToListAsync());

        public async Task<TreatmentLocation> GetTreatmentLocation(string value)
            => (await ListTreatmentLocations())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<Ontology> GetOntology(string value)
            => (await ListOntologies())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<Sex>> ListSexes()
            => await CacheGetOrCreateWithAbsoluteExpiry(
                CacheKeys.Sexes,
                async () => await _db.Sexes.AsNoTracking()
                    .ToListAsync());

        public async Task<Sex> GetSex(string value)
            => (await ListSexes())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
    }
}