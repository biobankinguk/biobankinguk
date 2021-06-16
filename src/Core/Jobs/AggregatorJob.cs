using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Shared.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class AggregatorJob
    {
        private readonly IAggregationService _aggregationService;
        private readonly IReferenceDataService _refDataService;
        private readonly IOrganisationService _organisationService;
        private readonly ICollectionService _collectionService;
        private readonly ISampleService _sampleService;

        public AggregatorJob(
            IAggregationService aggregationService,
            IReferenceDataService refDataService,
            IOrganisationService organisationService,
            ICollectionService collectionService,
            ISampleService sampleService)
        {
            _aggregationService = aggregationService;
            _refDataService = refDataService;
            _organisationService = organisationService;
            _collectionService = collectionService;
            _sampleService = sampleService;
        }

        public async Task Run()
        {
            // All Samples Flagged For Update/Deletion
            var dirtyExtractedSamples = await _sampleService.ListDirtyExtractedSamples();

            // Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamples();

            // Group Samples Into Collections
            foreach (var collectionSamples in _aggregationService.GroupIntoCollections(dirtyExtractedSamples))
            {
                var sample = collectionSamples.First();
                var samples = await _sampleService.ListSimilarSamples(collectionSamples);
                var organisation = await _organisationService.GetById(sample.OrganisationId);

                // Find Exisiting Or Generate New Collection
                var collectionName = _aggregationService.GenerateCollectionName(sample);
                var collection =
                    await _collectionService.GetCollection(sample.OrganisationId, collectionName) ??
                    _aggregationService.GenerateCollection(samples.Any() ? samples : collectionSamples);

                if (samples.Any())
                {
                    // Update Collection Contextual Fields
                    collection.LastUpdated = DateTime.Now;
                    collection.CollectionTypeId = organisation.CollectionTypeId;
                    collection.AccessConditionId = organisation.AccessConditionId ?? 0;

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

                            // Set Collection Percentage For Material Detail
                            materialDetail.CollectionPercentage =
                                _refDataService.GetCollectionPercentage(
                                    100m * materialDetailSamples.Count() / sampleSetSamples.Count());

                            sampleSet.MaterialDetails.Add(materialDetail);
                        }

                        collection.SampleSets.Add(sampleSet);
                    }

                    // Write Collection To DB
                    if (collection.CollectionId == default)
                    {
                        await _collectionService.AddCollection(collection);
                    }
                    else
                    {
                        await _collectionService.UpdateCollection(collection);
                    }
                }
                else
                {
                    await _collectionService.DeleteMaterialDetailsBySampleSetIds(collection.SampleSets.Select(x => x.Id));
                    await _collectionService.DeleteSampleSetByIds(collection.SampleSets.Select(x => x.Id));
                    await _collectionService.DeleteCollection(collection.CollectionId);
                }

                // Flag These Samples As Clean
                await _sampleService.CleanSamples(samples);
            }
        }
    }
}
