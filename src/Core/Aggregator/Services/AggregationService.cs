using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IReferenceDataService _refDataService;
        private readonly BiobanksDbContext _db;
        
        public AggregationService(IReferenceDataService refDataService, BiobanksDbContext db)
        {
            _refDataService = refDataService;
            _db = db;
        }

        public IEnumerable<IEnumerable<LiveSample>> GroupIntoCollections(IEnumerable<LiveSample> samples)
        {
            return samples
                .GroupBy(x => new
                {
                    x.OrganisationId,
                    x.CollectionName,
                    x.SampleContentId
                })
                .Select(x => x.AsEnumerable());
        }

        public IEnumerable<IEnumerable<LiveSample>> GroupIntoSampleSets(IEnumerable<LiveSample> samples)
        {
            var defaultAgeRange = _refDataService.GetDefaultAgeRange();
            var ageRanges = _db.AgeRanges.Where(x => x.Id != defaultAgeRange.Id).ToList();

            // Group Samples Into SampleSets
            return samples
                .Select(sample => new
                {
                    Sample = sample,
                    AgeRange = ageRanges.FirstOrDefault(y => y.ContainsTimeSpan(sample.AgeAtDonation)) ?? defaultAgeRange
                })
                .GroupBy(x => new
                {
                    x.AgeRange.Id,
                    x.Sample.SexId
                })
                .Select(x => x.Select(y => y.Sample));
        }

        public IEnumerable<IEnumerable<LiveSample>> GroupIntoMaterialDetails(IEnumerable<LiveSample> samples)
        {
            return samples
                .Where(x => x.StorageTemperatureId != null) // TODO: How should this be handled
                .GroupBy(x => new
                {
                    x.MaterialTypeId,
                    x.StorageTemperatureId
                })
                .Select(x => x.AsEnumerable());
        }

        public Collection GenerateCollection(IEnumerable<LiveSample> samples)
        {
            var orderedSamples = samples.OrderBy(y => y.DateCreated);
            var oldestSample = orderedSamples.First();
            var newestSample = orderedSamples.Last();

            // Collection Complete If Newest Sample Older Than ~6 Months
            var complete = (DateTime.Now - newestSample.DateCreated) > TimeSpan.FromDays(180);

            // Generate Collection Name
            var collectionName = GenerateCollectionName(orderedSamples.First(x => x.SampleContentId != default)); 

            return new Collection
            {
                OrganisationId = newestSample.OrganisationId,
                Title = collectionName,
                OntologyTermId = newestSample.SampleContentId,
                StartDate = oldestSample.DateCreated,
                CollectionStatusId = _refDataService.GetCollectionStatus(complete).Id,
                FromApi = true,
                SampleSets = new List<SampleSet>()
            };
        }

        public SampleSet GenerateSampleSet(IEnumerable<LiveSample> samples)
        {
            var sample = samples.First();
            var ageRange = _refDataService.GetAgeRange(sample.AgeAtDonation);
            var donorCount = _refDataService.GetDonorCount(samples.Count());

            return new SampleSet
            {
                SexId = sample.SexId ?? 0, // TODO: Do we need a default Sex?
                AgeRangeId = ageRange.Id,
                DonorCountId = donorCount.Id,
                MaterialDetails = new List<MaterialDetail>()
            };
        }

        public MaterialDetail GenerateMaterialDetail(IEnumerable<LiveSample> samples)
        {
            var sample = samples.First();

            // TODO: Is there a better way rather than using hardcoded values?
            // Map Macroscopic Assessment
            //var macro = 
            //    sample.SampleContentMethod.Value.StartsWith("Microscopic") || 
            //    sample.SampleContentMethod.Value.StartsWith("Macroscopic")
            //        ? sample.SampleContent.Id == "102499006" || // Fit and Healthy
            //          sample.SampleContent.Id == "23875004"     // No pathelogical diagnosis
            //            ? _db.MacroscopicAssessments.First(x => x.Value.StartsWith("Not Affected"))
            //            : _db.MacroscopicAssessments.First(x => x.Value.StartsWith("Affected"))
            //        : _db.MacroscopicAssessments.First(x => x.Value.StartsWith("Not Applicable"));

            // TODO: Sort Out Non-Extracted Samples
            var macro = _db.MacroscopicAssessments.First(x => x.Value.StartsWith("Not Applicable"));

            return new MaterialDetail
            {
                MaterialTypeId = sample.MaterialTypeId,
                StorageTemperatureId = sample.StorageTemperatureId ?? 0,
                MacroscopicAssessmentId = macro.Id,
                ExtractionProcedureId = sample.ExtractionProcedureId,
                PreservationTypeId = sample.PreservationTypeId,
            };
        }

        public string GenerateCollectionName(LiveSample sample)
            => string.IsNullOrEmpty(sample.CollectionName)
                ? $"{sample.SampleContent.Value}"
                : $"{sample.CollectionName} ({sample.SampleContent.Value})";

    }
}
