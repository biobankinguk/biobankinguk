using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Submissions.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.Api;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IReferenceDataAggregatorService _refDataService;

        private readonly AggregatorOptions _options;
        private readonly ApplicationDbContext _db;
        
        public AggregationService(
            IReferenceDataAggregatorService refDataService,
            IOptions<AggregatorOptions> options,
            ApplicationDbContext db)
        {
            _refDataService = refDataService;
            _options = options.Value;
            _db = db;
        }

        public IEnumerable<IEnumerable<LiveSample>> GroupIntoCollections(IEnumerable<LiveSample> samples)
        {
            var nonExtracted = samples.All(x => string.IsNullOrEmpty(x.SampleContentId));

            if (nonExtracted)
            {
                return samples
                    .GroupBy(x => x.OrganisationId)
                    .Select(x => x.AsEnumerable());
            }
            else
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

        public Collection GenerateCollection(IEnumerable<LiveSample> samples, string collectionName)
        {
            var orderedSamples = samples.OrderBy(y => y.DateCreated);
            var oldestSample = orderedSamples.First();
            var newestSample = orderedSamples.Last();

            // Collection Complete If Newest Sample Older Than ~6 Months
            var complete = (DateTime.Now - newestSample.DateCreated) > TimeSpan.FromDays(180);

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
            var contentId = sample.SampleContentId;
            var contentMethod = sample.SampleContentMethod?.Value ?? "";

            // Map Macroscopic Assessment
            var macro = _options.MapToMacroscopicAssessment(contentMethod, contentId);
            var macroAssessment = _db.MacroscopicAssessments.First(x => x.Value == macro);

            return new MaterialDetail
            {
                MaterialTypeId = sample.MaterialTypeId,
                StorageTemperatureId = sample.StorageTemperatureId ?? 0,
                MacroscopicAssessmentId = macroAssessment.Id,
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
