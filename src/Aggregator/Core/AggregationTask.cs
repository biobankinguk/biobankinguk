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
            // All Samples Flagged For Update/Deletion
            var dirtySamples = await _sampleService.ListDirtySamplesAsync();

            // Delete Samples With isDeleted Flag
            await _sampleService.DeleteFlaggedSamplesAsync();

            foreach (var groupedSamples in _aggregationService.GroupIntoCollections(dirtySamples))
            {
                var samples = await _sampleService.ListSimilarSamplesAsync(groupedSamples.First());
                var collection = await _aggregationService.GenerateCollection(samples);

                if (samples.Any())
                {
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
