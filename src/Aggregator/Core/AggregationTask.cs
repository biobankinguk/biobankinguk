using Biobanks.Aggregator.Core.Services.Contracts;
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
            var dirtySamples = await _sampleService.ListDirtySamplesAsync();

            // Group Into New/Exisiting Collections
            var collections = await _aggregationService.GroupCollections(dirtySamples);

            // Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamplesAsync();

            // Build Collections
            foreach (var collection in collections)
            {
                var samples = await _sampleService.ListRelevantSamplesAsync(collection);

                if (samples.Any())
                {
                    // Generate and Populate SampleSets
                    collection.SampleSets = (await _aggregationService.GroupSampleSets(samples)).ToList();

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
            }
        }
    }
}
