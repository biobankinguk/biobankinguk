﻿using Biobanks.Aggregator.Core.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

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
                var sample = collectionSamples.First();
                var samples = await _sampleService.ListSimilarSamplesAsync(sample);

                // Find Exisiting Or Generate New Collection
                var collection =
                    await _collectionService.GetCollectionAsync(sample.OrganisationId, sample.CollectionName) ??
                    _aggregationService.GenerateCollection(samples.Any() ? samples : collectionSamples);

                if (samples.Any())
                {
                    // Clear Current SampleSets - Rebuilt Below
                    collection.SampleSets.Clear();

                    // Group Samples Into SampleSets
                    foreach (var sampleSetSamples in _aggregationService.GroupIntoSampleSets(samples))
                    {
                        var sampleSet = _aggregationService.GenerateSampleSet(sampleSetSamples);

                        // Group Samples Into MaterialDetails
                        foreach (var materialDetailSamples in _aggregationService.GroupIntoMaterialDetails(sampleSetSamples))
                        {
                            sampleSet.MaterialDetails.Add(_aggregationService.GenerateMaterialDetail(materialDetailSamples));
                        }

                        collection.SampleSets.Add(sampleSet);
                    }

                    // Write Collection To DB
                    if (collection.CollectionId == default)
                    {
                        await _collectionService.AddCollectionAsync(collection);
                    }
                    else
                    {
                        await _collectionService.UpdateCollectionAsync(collection);
                    }
                }
                else
                {
                    await _collectionService.DeleteCollectionAsync(collection);
                }

                // Flag These Samples As Clean
                await _sampleService.CleanSamplesAsync(collectionSamples);
            }
        }
    }
}
