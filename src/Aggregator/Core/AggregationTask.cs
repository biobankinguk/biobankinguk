using Biobanks.Aggregator.Core.Services.Contracts;
using System;
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
        private readonly IReferenceDataService _refDataService;
        private readonly ICollectionService _collectionService;
        private readonly ISampleService _sampleService;

        public AggregationTask(
            IAggregationService aggregationService,
            IReferenceDataService refDataService,
            ICollectionService collectionService,
            ISampleService sampleService)
        {
            _aggregationService = aggregationService;
            _refDataService = refDataService;
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
                    // List of collection sampleSets before clear()
                    var oldSampleSets = new List<int>();
                    foreach (var ss in collection.SampleSets)
                    {
                        oldSampleSets.Add(ss.Id);
                    }

                    // Clear Current SampleSets - Rebuilt Below
                    collection.SampleSets.Clear();

                    // Group Samples Into SampleSets
                    foreach (var sampleSetSamples in _aggregationService.GroupIntoSampleSets(samples))
                    {
                        var sampleSet = _aggregationService.GenerateSampleSet(sampleSetSamples);

                        // Group Samples Into MaterialDetails
                        foreach (var materialDetailSamples in _aggregationService.GroupIntoMaterialDetails(sampleSetSamples))
                        {
                            var materialDetail = _aggregationService.GenerateMaterialDetail(materialDetailSamples);
                            var percentage = decimal.Divide(sampleSetSamples.Count(), materialDetailSamples.Count());

                            materialDetail.CollectionPercentageId = _refDataService.GetCollectionPercentage(percentage).Id;

                            sampleSet.MaterialDetails.Add(materialDetail);
                        }

                        collection.SampleSets.Add(sampleSet);
                    }

                    // Update Timestamp
                    collection.LastUpdated = DateTime.Now;

                    // Write Collection To DB
                    if (collection.CollectionId == default)
                    {
                        await _collectionService.AddCollectionAsync(collection);
                    }
                    else
                    {
                        foreach (var ss in oldSampleSets.Distinct())
                        {
                            await _aggregationService.DeleteMaterialDetailsBySampleSetId(ss);
                        }
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

                // Flag These Samples As Clean
                await _sampleService.CleanSamplesAsync(collectionSamples);
            }
        }
    }
}
