using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly BiobanksDbContext _db;

        public AggregationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SampleSet>> GroupSampleSets(IEnumerable<LiveSample> samples)
        {
            var donorCounts = await _db.DonorCounts.ToListAsync();
            var ageRanges = await _db.AgeRanges.ToListAsync();
            var defaultAgeRange = ageRanges.FirstOrDefault(x => x.LowerBound == null && x.UpperBound == null);

            // TODO: Some Error Logging If No Default Value Exists?
            if (defaultAgeRange is null)
            {
            }

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
                .Select(x => 
                {
                    var sample = x.First().Sample;
                    var ageRange = x.First().AgeRange;

                    // TODO: Error Handling - Possible DonorCount Bracket Doesn't Exist
                    var donorCount = donorCounts.First(y => 
                        y.LowerBound <= x.Count() && 
                        y.UpperBound >= x.Count());

                    return new SampleSet
                    {
                        SexId = sample.SexId ?? 0, // TODO: Do we need a default Sex?
                        AgeRangeId = ageRange.Id,
                        DonorCountId = donorCount.Id
                    };
                });
        }

        public async Task<IEnumerable<Collection>> GroupCollections(IEnumerable<LiveSample> samples)
        {
            // Currently Only Supports Extracted Samples
            var extractedMaterialGroups = await _db.MaterialTypeGroups.Where(x => x.Value.StartsWith("Extracted")).ToListAsync();
            var extractedSamples = samples.Where(x => x.MaterialType.MaterialTypeGroups.Any(x => extractedMaterialGroups.Contains(x)));

            // Grouping Of Samples
            var collections = extractedSamples.GroupBy(x => 
                new 
                { 
                    x.OrganisationId, 
                    x.CollectionName,
                    x.SampleContentId
                })
                .Select(group => group.First())
                .Select(sample => 
                {
                    var collection = _db.Collections.FirstOrDefault(x =>
                        x.OrganisationId == sample.OrganisationId &&
                        x.Title == sample.CollectionName &&
                        x.FromApi
                    );

                    return collection ?? new Collection
                    {
                        OrganisationId = sample.OrganisationId,
                        Title = sample.CollectionName,
                        FromApi = true
                    };
                });

            return collections;
        }

        public Task<IEnumerable<MaterialDetail>> GenerateMaterialDetails(IEnumerable<LiveSample> samples)
        {
            throw new System.NotImplementedException();
        }
    }
}
