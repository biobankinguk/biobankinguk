using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly ICollectionService _collectionService;
        private readonly BiobanksDbContext _db;
        
        public AggregationService(ICollectionService collectionService, BiobanksDbContext db)
        {
            _collectionService = collectionService;
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
            var defaultAgeRange = GetDefaultAgeRange();

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

        public async Task<Collection> GenerateCollection(IEnumerable<LiveSample> samples)
        {
            var orderedSamples = samples.OrderBy(y => y.DateCreated);
            var newestSample = orderedSamples.Last();

            // Collection Complete If Newest Sample Older Than ~6 Months
            var complete = (DateTime.Now - newestSample.DateCreated) > TimeSpan.FromDays(180);

            // Find Exisiting Collection
            var collection = await _collectionService.GetCollectionAsync(
                    newestSample.OrganisationId, 
                    newestSample.CollectionName)
                ?? new Collection
                {
                    OrganisationId = newestSample.OrganisationId,
                    Title = newestSample.CollectionName,
                    //OntologyTermId
                    //Description
                    StartDate = orderedSamples.First().DateCreated,
                    HtaStatusId = GetDefaultHtaStatus().Id,   // TODO: Needs Deleting?
                    AccessConditionId = GetDefaultAccessCondition().Id, // TODO: Temp Value
                    CollectionTypeId = GetDefaultCollectionType().Id, // TODO: Temp Value
                    CollectionStatusId = GetCollectionStatus(complete).Id,
                    CollectionPointId = GetDefaultCollectionPoint().Id, // TODO: Needs Deleting?
                    FromApi = true,
                    SampleSets = new List<SampleSet>()
                };

            // Set LastUpdated Timestamp
            collection.LastUpdated = DateTime.Now;

            return collection;
        }

        public async Task<SampleSet> GenerateSampleSet(IEnumerable<LiveSample> samples)
        {
            var sample = samples.First();
            var ageRange = GetAgeRange(sample.AgeAtDonation);
            var donorCount = GetDonorCount(samples.Count());

            return new SampleSet
            {
                SexId = sample.SexId ?? 0, // TODO: Do we need a default Sex?
                AgeRangeId = ageRange.Id,
                DonorCountId = donorCount.Id,
                MaterialDetails = new List<MaterialDetail>()
            };
        }

        public async Task<MaterialDetail> GenerateMaterialDetail(IEnumerable<LiveSample> samples)
        {
            var sample = samples.First();

            return new MaterialDetail
            {
                MaterialTypeId = sample.MaterialTypeId,
                StorageTemperatureId = sample.StorageTemperatureId ?? 0,
                MacroscopicAssessmentId = 3,  // TODO: Mapping rule unknown
                ExtractionProcedureId = sample.ExtractionProcedureId,
                PreservationTypeId = sample.PreservationTypeId,
                CollectionPercentageId = GetCollectionPercentage(0).Id
            };
        }

        // RefData Helper - TODO: Put in own service?
        private AgeRange GetAgeRange(string age)
            => _db.AgeRanges.ToList().FirstOrDefault(y =>
                    XmlConvert.ToTimeSpan(y.LowerBound) <= XmlConvert.ToTimeSpan(age) &&
                    XmlConvert.ToTimeSpan(y.UpperBound) >= XmlConvert.ToTimeSpan(age)) ?? GetDefaultAgeRange();

        private CollectionPercentage GetCollectionPercentage(decimal percentage)
            => _db.CollectionPercentages.FirstOrDefault(y =>
                    y.LowerBound <= percentage &&
                    y.UpperBound >= percentage);

        private DonorCount GetDonorCount(int count)
            => _db.DonorCounts.First(x => x.LowerBound <= count && x.UpperBound >= count);

        private CollectionStatus GetCollectionStatus(bool complete)
        {
            return complete
                ? _db.CollectionStatus.Where(x => x.Value == "Completed").First()
                : _db.CollectionStatus.Where(x => x.Value == "In progress").First();
        }

        private AccessCondition GetDefaultAccessCondition()
            => _db.AccessConditions.OrderBy(x => x.SortOrder).First();

        private CollectionType GetDefaultCollectionType()
            => _db.CollectionTypes.OrderBy(x => x.SortOrder).First();

        private CollectionPoint GetDefaultCollectionPoint()
            => _db.CollectionPoints.OrderBy(x => x.SortOrder).First();

        private HtaStatus GetDefaultHtaStatus()
            => _db.HtaStatus.OrderBy(x => x.SortOrder).First();

        private AgeRange GetDefaultAgeRange()
            => _db.AgeRanges.First(x => x.LowerBound == null && x.UpperBound == null);
    }
}
