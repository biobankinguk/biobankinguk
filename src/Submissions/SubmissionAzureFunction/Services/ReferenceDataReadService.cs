﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Common.Types;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Biobanks.LegacyData;

using MaterialType = Biobanks.LegacyData.Entities.MaterialType;
using MaterialTypeGroup = Biobanks.LegacyData.Entities.MaterialTypeGroup;
using MaterialTypeMaterialTypeGroup = Biobanks.LegacyData.Entities.JoinEntities.MaterialTypeMaterialTypeGroup;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.SubmissionAzureFunction.Services
{
    public class ReferenceDataReadService : IReferenceDataReadService
    {
        private readonly IMemoryCache _cache;
        private readonly BiobanksDbContext _db;

        public ReferenceDataReadService(
            IMemoryCache cache, BiobanksDbContext db)
        {
            _cache = cache;
            _db = db;
        }

        public async Task<IEnumerable<SnomedTag>> ListSnomedTags()
        {
            if (_cache.TryGetValue(CacheKeys.SnomedTags, out IEnumerable<SnomedTag> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.SnomedTags.AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.SnomedTags, cacheEntry);

            return cacheEntry;
        }

        private async Task<IEnumerable<SnomedTag>> GetSnomedTagsByValues(IReadOnlyCollection<string> values)
            => (await ListSnomedTags())
                .Where(x => values.Contains(x.Value));

        public async Task<IEnumerable<OntologyTerm>> ListOntologyTerms()
        {
            if (_cache.TryGetValue(CacheKeys.OntologyTerms, out IEnumerable<OntologyTerm> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.OntologyTerms.Include(x => x.SnomedTag).AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.OntologyTerms, cacheEntry);

            return cacheEntry;
        }

        public async Task<IEnumerable<Ontology>> ListOntologies()
        {
            if (_cache.TryGetValue(CacheKeys.Ontologies, out IEnumerable<Ontology> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.Ontologies.Include(x => x.OntologyVersions).AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.Ontologies, cacheEntry);

            return cacheEntry;
        }

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
            => await SnomedLookup(value, field, new[] {"Disease"});

        public async Task<OntologyTerm> GetSnomedTreatment(string value, string field)
            => await SnomedLookup(value, field, new[] {"Treatment"});

        public async Task<OntologyTerm> GetSnomedBodyOrgan(string value, string field)
            => await SnomedLookup(value, field, new[] {"Body Organ"});

        public async Task<OntologyTerm> GetSnomedExtractionProcedure(string value, string field)
            => await SnomedLookup(value, field, new[] {"Extraction Procedure"});

        public async Task<IEnumerable<MaterialType>> ListMaterialTypes()
        {
            if (_cache.TryGetValue(CacheKeys.MaterialTypes, out IEnumerable<MaterialType> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.MaterialTypes
                .AsNoTracking()
                .ToListAsync();

            _cache.Set(CacheKeys.MaterialTypes, cacheEntry);

            return cacheEntry;
        }

        public async Task<MaterialType> GetMaterialTypeWithGroups(string value)
            => (await ListMaterialTypes())
                .FirstOrDefault(x => 
                    (x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase)) &&
                    (x.MaterialTypeGroups?.Any() ?? false));

        public async Task<IEnumerable<SampleContentMethod>> ListSampleContentMethods()
        {
            if (_cache.TryGetValue(CacheKeys.SampleContentMethods, out IEnumerable<SampleContentMethod> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.SampleContentMethods.AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.SampleContentMethods, cacheEntry);

            return cacheEntry;
        }

        public async Task<SampleContentMethod> GetSampleContentMethod(string value)
            => (await ListSampleContentMethods())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<StorageTemperature>> ListStorageTemperatures()
        {
            if (_cache.TryGetValue(CacheKeys.StorageTemperatures, out IEnumerable<StorageTemperature> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.StorageTemperatures.AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.StorageTemperatures, cacheEntry);

            return cacheEntry;
        }

        public async Task<StorageTemperature> GetStorageTemperature(string value)
            => (await ListStorageTemperatures())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<PreservationType>> ListPreservationTypes() 
            => await _db.PreservationTypes.AsNoTracking().ToListAsync();

        public async Task<PreservationType> GetPreservationType(string value)
            => (await ListPreservationTypes())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<TreatmentLocation>> ListTreatmentLocations()
        {
            if (_cache.TryGetValue(CacheKeys.TreatmentLocations, out IEnumerable<TreatmentLocation> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.TreatmentLocations.AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.TreatmentLocations, cacheEntry);

            return cacheEntry;
        }

        public async Task<TreatmentLocation> GetTreatmentLocation(string value)
            => (await ListTreatmentLocations())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<Ontology> GetOntology(string value)
            => (await ListOntologies())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));

        public async Task<IEnumerable<Sex>> ListSexes()
        {
            if (_cache.TryGetValue(CacheKeys.Sex, out IEnumerable<Sex> cacheEntry)) return cacheEntry;

            cacheEntry = await _db.Sexes.AsNoTracking().ToListAsync();

            _cache.Set(CacheKeys.Sex, cacheEntry);

            return cacheEntry;
        }

        public async Task<Sex> GetSex(string value)
            => (await ListSexes())
                .FirstOrDefault(x => x.Value.Equals(value, StringComparison.CurrentCultureIgnoreCase));
    }
}