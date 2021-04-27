using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
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
                .Select(x => x.AsEnumerable())
                .ToList();
        }

        public IEnumerable<IEnumerable<LiveSample>> GroupIntoSampleSets(IEnumerable<LiveSample> samples)
        {
            var ageRanges = _db.AgeRanges.ToList();
            var defaultAgeRange = _refDataService.GetDefaultAgeRange();

            // Group Samples Into SampleSets
            return samples
                .Select(sample => new
                {
                    Sample = sample,
                    AgeRange = ageRanges.FirstOrDefault(y =>
                        XmlConvert.ToTimeSpan(y.LowerBound) <= XmlConvert.ToTimeSpan(sample.AgeAtDonation) &&
                        XmlConvert.ToTimeSpan(y.UpperBound) >= XmlConvert.ToTimeSpan(sample.AgeAtDonation)) ?? defaultAgeRange
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
            var newestSample = orderedSamples.Last();

            // Collection Complete If Newest Sample Older Than ~6 Months
            var complete = (DateTime.Now - newestSample.DateCreated) > TimeSpan.FromDays(180);

            // Find Exisiting Collection
            return new Collection
            {
                OrganisationId = newestSample.OrganisationId,
                Title = newestSample.CollectionName,
                //OntologyTermId
                //Description
                StartDate = orderedSamples.First().DateCreated,
                HtaStatusId = _refDataService.GetDefaultHtaStatus().Id,   // TODO: Needs Deleting?
                AccessConditionId = _refDataService.GetDefaultAccessCondition().Id, // TODO: Temp Value
                CollectionTypeId = _refDataService.GetDefaultCollectionType().Id, // TODO: Temp Value
                CollectionStatusId = _refDataService.GetCollectionStatus(complete).Id,
                CollectionPointId = _refDataService.GetDefaultCollectionPoint().Id, // TODO: Needs Deleting?
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

            return new MaterialDetail
            {
                MaterialTypeId = sample.MaterialTypeId,
                StorageTemperatureId = sample.StorageTemperatureId ?? 0,
                MacroscopicAssessmentId = 3,  // TODO: Mapping rule unknown
                ExtractionProcedureId = sample.ExtractionProcedureId,
                PreservationTypeId = sample.PreservationTypeId,
                CollectionPercentageId = _refDataService.GetCollectionPercentage(0).Id // TODO: Map In AggregationTask
            };
        }
    }
}
