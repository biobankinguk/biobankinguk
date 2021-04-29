using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Biobanks.Aggregator.Core
{
    public class AggregationTask
    {
        private readonly IAggregationService _aggregationService;
        private readonly ICollectionService _collectionService;
        private readonly ISampleService _sampleService;

        public AggregationTask(
            IAggregationService aggregationService,
            ICollectionService collectionService,
            ISampleService sampleService)
        {
            _aggregationService = aggregationService;
            _collectionService = collectionService;
            _sampleService = sampleService;
        }

        public async Task Run()
        {
            // All Samples Flagged For Update/Deletion
            var dirtySamples = await _sampleService.ListDirtySamplesAsync();

            // Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamplesAsync();

            // Group Samples Into Collections
            foreach (var collectionSamples in _aggregationService.GroupIntoCollections(dirtySamples))
            {
                var samples = await _sampleService.ListSimilarSamplesAsync(collectionSamples.First());
                var collection = await _aggregationService.GenerateCollection(samples.Any() ? samples : collectionSamples);

                if (samples.Any())
                {
                    // List of collection sampleSets before clear()
                    var oldSampleSets = new List<int>();
                    foreach (var ss in collection.SampleSets)
                    {
                        oldSampleSets.Add(ss.Id);
                    }

                    // Clear Current SampleSets - Rebuilt Below
                    collection.SampleSets.Clear();

                    // Group Into SampleSets
                    foreach (var sampleSetSamples in _aggregationService.GroupIntoSampleSets(samples))
                    {
                        var sampleSet = await _aggregationService.GenerateSampleSet(sampleSetSamples);

                        collection.SampleSets.Add(sampleSet);
                    }

                    if (collection.CollectionId == default)
                    {
                        await _collectionService.AddCollectionAsync(collection);
                    }
                    else
                    {
                        await _collectionService.UpdateCollectionAsync(collection);

                        // Remove old sampleSets from db if present
                        foreach (var ss in oldSampleSets.Distinct())
                        {
                            await _sampleService.DeleteSampleSetById(ss);
                        }
                    }
                }
                else
                {
                    await _collectionService.DeleteCollectionAsync(collection);
                }
            }
        }
    }
}
